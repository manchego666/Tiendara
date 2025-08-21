// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Repo de Chat con columna CreadoEn normalizada.
// Fecha: 2025-08-19
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaSql.Conexion;

namespace Tiendara.CapaSql.ChatRepo
{
    public class ChatServiceSql : IChatRepo
    {
        private readonly string _cs = ConfiguracionSql.ConnectionString;

        public async Task<Guid> EnsureThreadAsync(Guid remitenteId, bool remitenteEsTienda, Guid destinatarioId, bool destinatarioEsTienda)
        {
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();

            // 1) Buscar en cualquier orden
            var id = await FindThreadAsync(cn, remitenteId, remitenteEsTienda, destinatarioId, destinatarioEsTienda)
                     ?? await FindThreadAsync(cn, destinatarioId, destinatarioEsTienda, remitenteId, remitenteEsTienda);
            if (id.HasValue) return id.Value;

            // 2) Insertar (si compite, reintenta lectura)
            var nuevo = Guid.NewGuid();
            const string ins = @"
INSERT INTO dbo.ChatThread(Id, AId, AEsTienda, BId, BEsTienda, UltimoMensajeEn)
VALUES(@Id,@AId,@AEsTienda,@BId,@BEsTienda,SYSUTCDATETIME());";

            try
            {
                using var cmd = new SqlCommand(ins, cn);
                cmd.Parameters.AddWithValue("@Id", nuevo);
                cmd.Parameters.AddWithValue("@AId", remitenteId);
                cmd.Parameters.AddWithValue("@AEsTienda", remitenteEsTienda);
                cmd.Parameters.AddWithValue("@BId", destinatarioId);
                cmd.Parameters.AddWithValue("@BEsTienda", destinatarioEsTienda);
                await cmd.ExecuteNonQueryAsync();
                return nuevo;
            }
            catch (SqlException)
            {
                var again = await FindThreadAsync(cn, remitenteId, remitenteEsTienda, destinatarioId, destinatarioEsTienda)
                            ?? await FindThreadAsync(cn, destinatarioId, destinatarioEsTienda, remitenteId, remitenteEsTienda);
                if (again.HasValue) return again.Value;
                throw;
            }
        }

        public async Task AddMensajeAsync(Guid threadId, Guid autorId, bool autorEsTienda, string texto, string? mediaUrl = null)
        {
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var tx = cn.BeginTransaction();

            const string ins = @"
INSERT INTO dbo.Mensaje(Id, ThreadId, AutorId, AutorEsTienda, Texto, MediaUrl, CreadoEn)
VALUES(@Id,@ThreadId,@AutorId,@AutorEsTienda,@Texto,@MediaUrl,SYSUTCDATETIME());";

            using (var cmd = new SqlCommand(ins, cn, tx))
            {
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@ThreadId", threadId);
                cmd.Parameters.AddWithValue("@AutorId", autorId);
                cmd.Parameters.AddWithValue("@AutorEsTienda", autorEsTienda);
                cmd.Parameters.AddWithValue("@Texto", texto);
                cmd.Parameters.AddWithValue("@MediaUrl", (object?)mediaUrl ?? DBNull.Value);
                await cmd.ExecuteNonQueryAsync();
            }

            const string upd = "UPDATE dbo.ChatThread SET UltimoMensajeEn=SYSUTCDATETIME() WHERE Id=@Id;";
            using (var cmd2 = new SqlCommand(upd, cn, tx))
            {
                cmd2.Parameters.AddWithValue("@Id", threadId);
                await cmd2.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
        }

        public async Task<IReadOnlyList<Mensaje>> ListMensajesAsync(Guid threadId, int top = 100, DateTime? antesDe = null)
        {
            var sql = "SELECT TOP(@Top) * FROM dbo.Mensaje WHERE ThreadId=@ThreadId";
            if (antesDe.HasValue) sql += " AND CreadoEn<@Antes";
            sql += " ORDER BY CreadoEn DESC;";

            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Top", top);
            cmd.Parameters.AddWithValue("@ThreadId", threadId);
            if (antesDe.HasValue) cmd.Parameters.AddWithValue("@Antes", antesDe.Value);

            using var rd = await cmd.ExecuteReaderAsync();
            var list = new List<Mensaje>();
            while (await rd.ReadAsync())
            {
                list.Add(new Mensaje
                {
                    Id = (Guid)rd["Id"],
                    ThreadId = (Guid)rd["ThreadId"],
                    AutorId = (Guid)rd["AutorId"],
                    AutorEsTienda = (bool)rd["AutorEsTienda"],
                    Texto = (string)rd["Texto"],
                    MediaUrl = rd["MediaUrl"] as string,
                    CreadoEn = (DateTime)rd["CreadoEn"]
                });
            }
            return list;
        }

        public async Task<IReadOnlyList<ChatThread>> InboxAsync(Guid sujetoId, bool esTienda, int top = 50)
        {
            const string sql = @"
SELECT TOP(@Top) * FROM dbo.ChatThread
WHERE (AId=@Id AND AEsTienda=@EsTienda) OR (BId=@Id AND BEsTienda=@EsTienda)
ORDER BY UltimoMensajeEn DESC;";

            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Top", top);
            cmd.Parameters.AddWithValue("@Id", sujetoId);
            cmd.Parameters.AddWithValue("@EsTienda", esTienda);

            using var rd = await cmd.ExecuteReaderAsync();
            var list = new List<ChatThread>();
            while (await rd.ReadAsync())
            {
                list.Add(new ChatThread
                {
                    Id = (Guid)rd["Id"],
                    AId = (Guid)rd["AId"],
                    AEsTienda = (bool)rd["AEsTienda"],
                    BId = (Guid)rd["BId"],
                    BEsTienda = (bool)rd["BEsTienda"],
                    UltimoMensajeEn = (DateTime)rd["UltimoMensajeEn"]
                });
            }
            return list;
        }

        private static async Task<Guid?> FindThreadAsync(SqlConnection cn, Guid AId, bool AEsTienda, Guid BId, bool BEsTienda)
        {
            const string sel = @"
SELECT TOP(1) Id FROM dbo.ChatThread
WHERE AId=@AId AND AEsTienda=@AEsTienda AND BId=@BId AND BEsTienda=@BEsTienda;";
            using var cmd = new SqlCommand(sel, cn);
            cmd.Parameters.AddWithValue("@AId", AId);
            cmd.Parameters.AddWithValue("@AEsTienda", AEsTienda);
            cmd.Parameters.AddWithValue("@BId", BId);
            cmd.Parameters.AddWithValue("@BEsTienda", BEsTienda);
            var o = await cmd.ExecuteScalarAsync();
            return o is Guid g ? g : (Guid?)null;
        }
    }
}
