// ------------------------------------------------------------
// Proyecto: Tiendara
// Repo: Seguimiento (ISeguimientoRepo)
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Tiendara.CapaContratos;
using Tiendara.CapaSql.Conexion;

namespace Tiendara.CapaSql.SeguimientoRepo
{
    public sealed class SeguimientoServiceSql : ISeguimientoRepo
    {
        private readonly string _cs = ConfiguracionSql.ConnectionString;

        public async Task SeguirAsync(Guid seguidorUsuarioId, Guid targetId, bool targetEsTienda)
        {
            const string sql = @"
IF NOT EXISTS (
  SELECT 1 FROM dbo.Seguimiento WITH (UPDLOCK, HOLDLOCK)
  WHERE SeguidorUsuarioId=@Seguidor AND TargetId=@Target AND TargetEsTienda=@EsTienda
)
BEGIN
  INSERT INTO dbo.Seguimiento(SeguidorUsuarioId, TargetId, TargetEsTienda, CreadoEn)
  VALUES (@Seguidor, @Target, @EsTienda, SYSUTCDATETIME());
END";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Seguidor", seguidorUsuarioId);
            cmd.Parameters.AddWithValue("@Target", targetId);
            cmd.Parameters.AddWithValue("@EsTienda", targetEsTienda);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DejarSeguirAsync(Guid seguidorUsuarioId, Guid targetId, bool targetEsTienda)
        {
            const string sql = @"DELETE FROM dbo.Seguimiento WHERE SeguidorUsuarioId=@Seguidor AND TargetId=@Target AND TargetEsTienda=@EsTienda;";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Seguidor", seguidorUsuarioId);
            cmd.Parameters.AddWithValue("@Target", targetId);
            cmd.Parameters.AddWithValue("@EsTienda", targetEsTienda);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> SigueAsync(Guid seguidorUsuarioId, Guid targetId, bool targetEsTienda)
        {
            const string sql = @"SELECT 1 FROM dbo.Seguimiento WHERE SeguidorUsuarioId=@Seguidor AND TargetId=@Target AND TargetEsTienda=@EsTienda;";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Seguidor", seguidorUsuarioId);
            cmd.Parameters.AddWithValue("@Target", targetId);
            cmd.Parameters.AddWithValue("@EsTienda", targetEsTienda);
            var o = await cmd.ExecuteScalarAsync();
            return o is not null;
        }

        public async Task<IReadOnlyList<Guid>> ListSeguidoresAsync(Guid targetId, bool targetEsTienda)
        {
            const string sql = @"SELECT SeguidorUsuarioId FROM dbo.Seguimiento WHERE TargetId=@Target AND TargetEsTienda=@EsTienda;";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Target", targetId);
            cmd.Parameters.AddWithValue("@EsTienda", targetEsTienda);
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<Guid>();
            while (await rd.ReadAsync()) list.Add((Guid)rd["SeguidorUsuarioId"]);
            return list;
        }

        public async Task<IReadOnlyList<Guid>> ListSiguiendoAsync(Guid usuarioId)
        {
            const string sql = @"SELECT TargetId FROM dbo.Seguimiento WHERE SeguidorUsuarioId=@U;";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@U", usuarioId);
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<Guid>();
            while (await rd.ReadAsync()) list.Add((Guid)rd["TargetId"]);
            return list;
        }
    }
}
