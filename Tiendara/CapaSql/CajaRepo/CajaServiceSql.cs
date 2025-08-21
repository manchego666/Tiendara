// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Clase base para todas las entidades SQL que no tienen otra herencia.
// Fecha: 2025-08-19
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System.Data;
using Microsoft.Data.SqlClient;
using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaSql.Conexion;

namespace Tiendara.CapaSql.CajaRepo
{
    public class CajaServiceSql : ICajaRepo
    {
        private readonly string _cs = ConfiguracionSql.ConnectionString;

        public async Task AddMovimientoAsync(MovimientoCaja m)
        {
            if (m.Id == Guid.Empty) m.Id = Guid.NewGuid();
            const string sql = @"
INSERT INTO dbo.MovimientoCaja
(Id, NegocioId, Fecha, Tipo, Monto, Medio, Concepto, VentaId, Usuario)
VALUES
(@Id,@NegocioId,SYSUTCDATETIME(),@Tipo,@Monto,@Medio,@Concepto,@VentaId,@Usuario);";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", m.Id);
            cmd.Parameters.AddWithValue("@NegocioId", m.NegocioId);
            cmd.Parameters.AddWithValue("@Tipo", (int)m.Tipo);
            cmd.Parameters.AddWithValue("@Monto", m.Monto);
            cmd.Parameters.AddWithValue("@Medio", (int)m.Medio);
            cmd.Parameters.AddWithValue("@Concepto", (object?)m.Concepto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@VentaId", (object?)m.VentaId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Usuario", (object?)m.Usuario ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IReadOnlyList<MovimientoCaja>> ListMovimientosAsync(Guid negocioId, DateTime desde, DateTime hasta)
        {
            const string sql = @"
SELECT * FROM dbo.MovimientoCaja
WHERE NegocioId=@NegocioId AND Fecha>=@Desde AND Fecha<=@Hasta
ORDER BY Fecha DESC;";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@NegocioId", negocioId);
            cmd.Parameters.AddWithValue("@Desde", desde);
            cmd.Parameters.AddWithValue("@Hasta", hasta);
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<MovimientoCaja>();
            while (await rd.ReadAsync())
            {
                int ixConcepto = rd.GetOrdinal("Concepto");
                int ixVentaId = rd.GetOrdinal("VentaId");
                int ixUsuario = rd.GetOrdinal("Usuario");

                list.Add(new MovimientoCaja
                {
                    Id = (Guid)rd["Id"],
                    NegocioId = (Guid)rd["NegocioId"],
                    Fecha = (DateTime)rd["Fecha"],
                    Tipo = (TipoMovimientoCaja)(int)rd["Tipo"],
                    Monto = (decimal)rd["Monto"],
                    Medio = (MedioPago)(int)rd["Medio"],
                    Concepto = rd.IsDBNull(ixConcepto) ? null : rd.GetString(ixConcepto),
                    VentaId = rd.IsDBNull(ixVentaId) ? (Guid?)null : rd.GetGuid(ixVentaId),
                    Usuario = rd.IsDBNull(ixUsuario) ? null : rd.GetString(ixUsuario)
                });
            }

            return list;
        }
    }
}
