// ------------------------------------------------------------
// Proyecto: Tiendara
// Repo: Negocio (INegocioRepo)
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaSql.Conexion;

namespace Tiendara.CapaSql.NegocioRepo
{
    public sealed class NegocioServiceSql : INegocioRepo
    {
        private readonly string _cs = ConfiguracionSql.ConnectionString;

        public async Task EliminarAsync(Guid id)
        {
            const string sql = @"UPDATE dbo.Negocio SET Activo=0, ModificadoEn=SYSUTCDATETIME() WHERE Id=@Id;";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Negocio>> ObtenerTodosAsync()
        {
            const string sql = @"SELECT * FROM dbo.Negocio WHERE Activo=1 ORDER BY CreadoEn DESC;";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<Negocio>();
            while (await rd.ReadAsync()) list.Add(Map(rd));
            return list;
        }

        // Opcionalmente InsertarAsync / ActualizarAsync si quieres usar los nombres de ICrudSql
        public async Task InsertarAsync(Negocio n) => await AddAsync(n);
        public async Task ActualizarAsync(Negocio n) => await UpdateAsync(n);
        public async Task<Negocio?> ObtenerPorIdAsync(Guid id) => await GetByIdAsync(id);


        public async Task AddAsync(Negocio n)
        {
            if (n.Id == Guid.Empty) n.Id = Guid.NewGuid();
            const string sql = @"
INSERT INTO dbo.Negocio
(Id, PropietarioUsuarioId, Nombre, Giro, EstadoMarca, NombreMarca,
 Direccion, Latitud, Longitud, Telefono, FotoLogoPath, Notas,
 Abierto, Activo, CreadoEn)
VALUES
(@Id,@PropietarioUsuarioId,@Nombre,@Giro,@EstadoMarca,@NombreMarca,
 @Direccion,@Latitud,@Longitud,@Telefono,@FotoLogoPath,@Notas,
 @Abierto,1,SYSUTCDATETIME());";

            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            AddParams(cmd, n, includeId: true);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Negocio n)
        {
            const string sql = @"
UPDATE dbo.Negocio SET
 PropietarioUsuarioId=@PropietarioUsuarioId,
 Nombre=@Nombre,
 Giro=@Giro,
 EstadoMarca=@EstadoMarca,
 NombreMarca=@NombreMarca,
 Direccion=@Direccion,
 Latitud=@Latitud,
 Longitud=@Longitud,
 Telefono=@Telefono,
 FotoLogoPath=@FotoLogoPath,
 Notas=@Notas,
 Abierto=@Abierto,
 ModificadoEn=SYSUTCDATETIME()
WHERE Id=@Id AND Activo=1;";

            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            AddParams(cmd, n, includeId: true);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<Negocio?> GetByIdAsync(Guid id)
        {
            const string sql = @"SELECT TOP(1) * FROM dbo.Negocio WHERE Id=@Id AND Activo=1;";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var rd = await cmd.ExecuteReaderAsync();
            return await rd.ReadAsync() ? Map(rd) : null;
        }

        public async Task<IReadOnlyList<Negocio>> ListByUsuarioAsync(Guid propietarioId)
        {
            const string sql = @"SELECT * FROM dbo.Negocio WHERE PropietarioUsuarioId=@U AND Activo=1 ORDER BY CreadoEn DESC;";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@U", propietarioId);
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<Negocio>();
            while (await rd.ReadAsync()) list.Add(Map(rd));
            return list;
        }

        // ===== Helpers =====
        private static void AddParams(SqlCommand cmd, Negocio n, bool includeId)
        {
            if (includeId) cmd.Parameters.AddWithValue("@Id", n.Id);
            cmd.Parameters.AddWithValue("@PropietarioUsuarioId", n.PropietarioUsuarioId);
            cmd.Parameters.AddWithValue("@Nombre", n.Nombre);
            cmd.Parameters.AddWithValue("@Giro", (object?)n.Giro ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EstadoMarca", (int)n.EstadoMarca);
            cmd.Parameters.AddWithValue("@NombreMarca", (object?)n.NombreMarca ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", (object?)n.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Latitud", (object?)n.Latitud ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Longitud", (object?)n.Longitud ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", (object?)n.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FotoLogoPath", (object?)n.FotoLogoPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notas", (object?)n.Notas ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Abierto", n.Abierto);
        }

        private static Negocio Map(IDataRecord r) => new Negocio
        {
            Id = (Guid)r["Id"],
            PropietarioUsuarioId = (Guid)r["PropietarioUsuarioId"],
            Nombre = (string)r["Nombre"],
            Giro = r["Giro"] as string,
            EstadoMarca = (EstatusMarca)(int)r["EstadoMarca"],
            NombreMarca = r["NombreMarca"] as string,
            Direccion = r["Direccion"] as string,
            Latitud = r["Latitud"] as double?,
            Longitud = r["Longitud"] as double?,
            Telefono = r["Telefono"] as string,
            FotoLogoPath = r["FotoLogoPath"] as string,
            Notas = r["Notas"] as string,
            Abierto = (bool)r["Abierto"],
            Activo = (bool)r["Activo"],
            CreadoEn = (DateTime)r["CreadoEn"],
            ModificadoEn = r["ModificadoEn"] as DateTime?,
            UltimoCambioEstado = r["UltimoCambioEstado"] as DateTime?
        };
    }
}
