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

namespace Tiendara.CapaSql.VentaRepo
{
    public class VentaServiceSql : IVentaRepo
    {
        private readonly string _cs = ConfiguracionSql.ConnectionString;

        public async Task AddAsync(Venta v)
        {
            if (v.Id == Guid.Empty) v.Id = Guid.NewGuid();
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var tx = cn.BeginTransaction();

            const string insV = @"
INSERT INTO dbo.Venta
(Id,NegocioId,VendedorUsuarioId,Fecha,Subtotal,Impuestos,Total,MedioPago,PagoRecibido,Cambio,Referencia,Estado,CreadoEn)
VALUES
(@Id,@NegocioId,@VendedorUsuarioId,SYSUTCDATETIME(),@Subtotal,@Impuestos,@Total,@MedioPago,@PagoRecibido,@Cambio,@Referencia,@Estado,SYSUTCDATETIME());";
            using (var cmd = new SqlCommand(insV, cn, tx))
            {
                cmd.Parameters.AddWithValue("@Id", v.Id);
                cmd.Parameters.AddWithValue("@NegocioId", v.NegocioId);
                cmd.Parameters.AddWithValue("@VendedorUsuarioId", v.VendedorUsuarioId);
                cmd.Parameters.AddWithValue("@Subtotal", v.Subtotal);
                cmd.Parameters.AddWithValue("@Impuestos", v.Impuestos);
                cmd.Parameters.AddWithValue("@Total", v.Total);
                cmd.Parameters.AddWithValue("@MedioPago", (int)v.MedioPago);
                cmd.Parameters.AddWithValue("@PagoRecibido", (object?)v.PagoRecibido ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Cambio", (object?)v.Cambio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Referencia", (object?)v.Referencia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Estado", (int)v.Estado);
                await cmd.ExecuteNonQueryAsync();
            }

            const string insL = @"
INSERT INTO dbo.VentaLinea
(Id, VentaId, ItemId, Nombre, Unidad, Cantidad, PrecioUnitario, TasaImpuesto, Importe)
VALUES
(@Id, @VentaId, @ItemId, @Nombre, @Unidad, @Cantidad, @PrecioUnitario, @TasaImpuesto, @Importe);";
            foreach (var l in v.Lineas)
            {
                using var cmdL = new SqlCommand(insL, cn, tx);
                cmdL.Parameters.AddWithValue("@Id", Guid.NewGuid());
                cmdL.Parameters.AddWithValue("@VentaId", v.Id);
                cmdL.Parameters.AddWithValue("@ItemId", l.ItemId);
                cmdL.Parameters.AddWithValue("@Nombre", l.Nombre);
                cmdL.Parameters.AddWithValue("@Unidad", (int)l.Unidad);
                cmdL.Parameters.AddWithValue("@Cantidad", l.Cantidad);
                cmdL.Parameters.AddWithValue("@PrecioUnitario", l.PrecioUnitario);
                cmdL.Parameters.AddWithValue("@TasaImpuesto", l.TasaImpuesto);
                cmdL.Parameters.AddWithValue("@Importe", l.Importe);
                await cmdL.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
        }

        public async Task<Venta?> GetByIdAsync(Guid id)
        {
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();

            const string sv = "SELECT TOP(1) * FROM dbo.Venta WHERE Id=@Id;";
            using var cmd = new SqlCommand(sv, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var rd = await cmd.ExecuteReaderAsync();
            if (!await rd.ReadAsync()) return null;

            var v = MapVenta(rd);
            rd.Close();

            const string sl = "SELECT * FROM dbo.VentaLinea WHERE VentaId=@VentaId;";
            using var cmdL = new SqlCommand(sl, cn);
            cmdL.Parameters.AddWithValue("@VentaId", id);
            using var rd2 = await cmdL.ExecuteReaderAsync();
            while (await rd2.ReadAsync()) v.Lineas.Add(MapLinea(rd2));
            return v;
        }

        public async Task<IReadOnlyList<Venta>> ListByFechaAsync(Guid negocioId, DateTime desde, DateTime hasta)
        {
            const string sql = @"
SELECT * FROM dbo.Venta
WHERE NegocioId=@NegocioId AND Fecha>=@Desde AND Fecha<=@Hasta
ORDER BY Fecha DESC;";
            using var cn = new SqlConnection(_cs);
            await cn.OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@NegocioId", negocioId);
            cmd.Parameters.AddWithValue("@Desde", desde);
            cmd.Parameters.AddWithValue("@Hasta", hasta);
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<Venta>();
            while (await rd.ReadAsync()) list.Add(MapVenta(rd));
            return list;
        }

        private static Venta MapVenta(IDataRecord r) => new Venta
        {
            Id = (Guid)r["Id"],
            NegocioId = (Guid)r["NegocioId"],
            VendedorUsuarioId = (Guid)r["VendedorUsuarioId"],
            Fecha = (DateTime)r["Fecha"],
            Subtotal = (decimal)r["Subtotal"],
            Impuestos = (decimal)r["Impuestos"],
            Total = (decimal)r["Total"],
            MedioPago = (MedioPago)(int)r["MedioPago"],
            PagoRecibido = r["PagoRecibido"] as decimal?,
            Cambio = r["Cambio"] as decimal?,
            Referencia = r["Referencia"] as string,
            Estado = (EstadoVenta)(int)r["Estado"],
            CreadoEn = (DateTime)r["CreadoEn"],
            ModificadoEn = r["ModificadoEn"] as DateTime?
        };

        private static VentaLinea MapLinea(IDataRecord r) => new VentaLinea
        {
            ItemId = (Guid)r["ItemId"],
            Nombre = (string)r["Nombre"],
            Unidad = (UnidadVenta)(int)r["Unidad"],
            Cantidad = (decimal)r["Cantidad"],
            PrecioUnitario = (decimal)r["PrecioUnitario"],
            TasaImpuesto = (decimal)r["TasaImpuesto"],
            Importe = (decimal)r["Importe"]
        };
    }
}
