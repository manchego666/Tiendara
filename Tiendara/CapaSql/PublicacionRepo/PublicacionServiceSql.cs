// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Publicaciones con borrado lógico por Estado.
// Fecha: 2025-08-19
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaSql.Base;
using Tiendara.CapaSql.Conexion;

namespace Tiendara.CapaSql.PublicacionRepo
{
    public class PublicacionServiceSql : CrudSqlBase<Publicacion>, IPublicacionRepo
    {
        public PublicacionServiceSql() : base(ConfiguracionSql.ConnectionString) { }

        public override async Task<Publicacion?> ObtenerPorIdAsync(Guid id)
        {
            const string sql = "SELECT TOP(1) * FROM dbo.Publicacion WHERE Id=@Id AND Estado='Publicado';";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var rd = await cmd.ExecuteReaderAsync();
            return await rd.ReadAsync() ? Map(rd) : null;
        }

        public override async Task<List<Publicacion>> ObtenerTodosAsync()
        {
            const string sql = "SELECT TOP(200) * FROM dbo.Publicacion WHERE Estado='Publicado' ORDER BY CreadoEn DESC;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            using var rd = await cmd.ExecuteReaderAsync();
            var list = new List<Publicacion>();
            while (await rd.ReadAsync()) list.Add(Map(rd));
            return list;
        }

        public override async Task InsertarAsync(Publicacion e)
        {
            if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();
            const string sql = @"
INSERT INTO dbo.Publicacion
(Id, Type, Country, State, City, EsTienda, AutorId, TiendaId, Texto, ImagenPath, Estado, CreadoEn)
VALUES
(@Id,@Type,@Country,@State,@City,@EsTienda,@AutorId,@TiendaId,@Texto,@ImagenPath,@Estado,SYSUTCDATETIME());";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            AddParams(cmd, e, includeId: true);
            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task ActualizarAsync(Publicacion e)
        {
            const string sql = @"
UPDATE dbo.Publicacion SET
 Type=@Type, Country=@Country, State=@State, City=@City, EsTienda=@EsTienda,
 AutorId=@AutorId, TiendaId=@TiendaId, Texto=@Texto, ImagenPath=@ImagenPath, Estado=@Estado,
 ModificadoEn=SYSUTCDATETIME()
WHERE Id=@Id;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            AddParams(cmd, e, includeId: true);
            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task EliminarAsync(Guid id)
        {
            // Borrado lógico: marcar como Eliminado
            const string sql = @"UPDATE dbo.Publicacion SET Estado='Eliminado', ModificadoEn=SYSUTCDATETIME() WHERE Id=@Id;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<Publicacion>> ListByAutorAsync(Guid autorId, bool esTienda, int top = 50)
        {
            const string sql = @"
SELECT TOP(@Top) * FROM dbo.Publicacion
WHERE AutorId=@AutorId AND EsTienda=@EsTienda AND Estado='Publicado'
ORDER BY CreadoEn DESC;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Top", top);
            cmd.Parameters.AddWithValue("@AutorId", autorId);
            cmd.Parameters.AddWithValue("@EsTienda", esTienda);
            using var rd = await cmd.ExecuteReaderAsync();
            var list = new List<Publicacion>();
            while (await rd.ReadAsync()) list.Add(Map(rd));
            return list;
        }

        public async Task<IReadOnlyList<Publicacion>> FeedAsync(string country, string state, string city, PublicationType? type = null, int top = 100)
        {
            var sql = @"
SELECT TOP(@Top) * FROM dbo.Publicacion
WHERE Estado='Publicado'
  AND (@Country IS NULL OR Country=@Country)
  AND (@State   IS NULL OR State=@State)
  AND (@City    IS NULL OR City=@City)";
            if (type.HasValue) sql += " AND Type=@Type";
            sql += " ORDER BY CreadoEn DESC;";

            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Top", top);
            cmd.Parameters.AddWithValue("@Country", (object?)country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@State", (object?)state ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City", (object?)city ?? DBNull.Value);
            if (type.HasValue) cmd.Parameters.AddWithValue("@Type", (int)type.Value);

            using var rd = await cmd.ExecuteReaderAsync();
            var list = new List<Publicacion>();
            while (await rd.ReadAsync()) list.Add(Map(rd));
            return list;
        }

        private static void AddParams(SqlCommand cmd, Publicacion e, bool includeId)
        {
            if (includeId) cmd.Parameters.AddWithValue("@Id", e.Id);
            cmd.Parameters.AddWithValue("@Type", (int)e.Type);
            cmd.Parameters.AddWithValue("@Country", (object?)e.Location?.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@State", (object?)e.Location?.State ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City", (object?)e.Location?.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EsTienda", e.EsTienda);
            cmd.Parameters.AddWithValue("@AutorId", e.AutorId);
            cmd.Parameters.AddWithValue("@TiendaId", (object?)e.TiendaId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Texto", (object?)e.Texto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ImagenPath", (object?)e.ImagenPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(e.Estado) ? "Publicado" : e.Estado);
        }

        private static Publicacion Map(IDataRecord r) => new Publicacion
        {
            Id = (Guid)r["Id"],
            Type = (PublicationType)(int)r["Type"],
            Location = new Ubicacion
            {
                Country = r["Country"] as string,
                State = r["State"] as string,
                City = r["City"] as string
            },
            EsTienda = (bool)r["EsTienda"],
            AutorId = (Guid)r["AutorId"],
            TiendaId = r["TiendaId"] as Guid?,
            Texto = r["Texto"] as string,
            ImagenPath = r["ImagenPath"] as string,
            Estado = r["Estado"] as string ?? "Publicado",
            CreadoEn = (DateTime)r["CreadoEn"],
            ModificadoEn = r["ModificadoEn"] as DateTime?
        };
    }
}
