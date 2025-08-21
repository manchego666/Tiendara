// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Clase base para todas las entidades SQL que no tienen otra herencia.
// Fecha: 2025-08-19
// Derechos reservados © ZORRODEV - 2025
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

namespace Tiendara.CapaSql.InventarioRepo
{
    public class InventarioServiceSql : CrudSqlBase<Inventario>, IInventarioRepo
    {
        public InventarioServiceSql() : base(ConfiguracionSql.ConnectionString) { }

        // =========== CRUD base ===========

        public override async Task<Inventario?> ObtenerPorIdAsync(Guid id)
        {
            const string sql = @"SELECT TOP(1) * FROM dbo.Inventario WHERE Id=@Id AND Activo=1;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var rd = await cmd.ExecuteReaderAsync();
            return await rd.ReadAsync() ? Map(rd) : null;
        }

        public override async Task<List<Inventario>> ObtenerTodosAsync()
        {
            const string sql = @"SELECT * FROM dbo.Inventario WHERE Activo=1 ORDER BY CreadoEn DESC;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<Inventario>();
            while (await rd.ReadAsync()) list.Add(Map(rd));
            return list;
        }

        public override async Task InsertarAsync(Inventario e)
        {
            if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();

            const string sql = @"
INSERT INTO dbo.Inventario
(Id, NegocioId, ProductoId, LoteId, CantidadDisponible, CantidadReservada,
 Minimo, Maximo, Ubicacion, CostoPromedio, CostoUltimaCompra, CreadoEn, Activo)
VALUES
(@Id, @NegocioId, @ProductoId, @LoteId, @CantDisp, @CantRes,
 @Min, @Max, @Ubicacion, @CostoProm, @CostoUlt, SYSUTCDATETIME(), 1);";

            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", e.Id);
            cmd.Parameters.AddWithValue("@NegocioId", e.NegocioId);
            cmd.Parameters.AddWithValue("@ProductoId", e.ProductoId);
            cmd.Parameters.AddWithValue("@LoteId", DbNullIfNull(e.LoteId));
            cmd.Parameters.AddWithValue("@CantDisp", e.CantidadDisponible);
            cmd.Parameters.AddWithValue("@CantRes", e.CantidadReservada);
            cmd.Parameters.AddWithValue("@Min", e.Minimo);
            cmd.Parameters.AddWithValue("@Max", e.Maximo);
            cmd.Parameters.AddWithValue("@Ubicacion", DbNullIfNull(e.Ubicacion));
            cmd.Parameters.AddWithValue("@CostoProm", e.CostoPromedio);
            cmd.Parameters.AddWithValue("@CostoUlt", e.CostoUltimaCompra);
            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task ActualizarAsync(Inventario e)
        {
            const string sql = @"
UPDATE dbo.Inventario
SET NegocioId=@NegocioId,
    ProductoId=@ProductoId,
    LoteId=@LoteId,
    CantidadDisponible=@CantDisp,
    CantidadReservada=@CantRes,
    Minimo=@Min,
    Maximo=@Max,
    Ubicacion=@Ubicacion,
    CostoPromedio=@CostoProm,
    CostoUltimaCompra=@CostoUlt,
    ModificadoEn=SYSUTCDATETIME()
WHERE Id=@Id AND Activo=1;";

            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", e.Id);
            cmd.Parameters.AddWithValue("@NegocioId", e.NegocioId);
            cmd.Parameters.AddWithValue("@ProductoId", e.ProductoId);
            cmd.Parameters.AddWithValue("@LoteId", DbNullIfNull(e.LoteId));
            cmd.Parameters.AddWithValue("@CantDisp", e.CantidadDisponible);
            cmd.Parameters.AddWithValue("@CantRes", e.CantidadReservada);
            cmd.Parameters.AddWithValue("@Min", e.Minimo);
            cmd.Parameters.AddWithValue("@Max", e.Maximo);
            cmd.Parameters.AddWithValue("@Ubicacion", DbNullIfNull(e.Ubicacion));
            cmd.Parameters.AddWithValue("@CostoProm", e.CostoPromedio);
            cmd.Parameters.AddWithValue("@CostoUlt", e.CostoUltimaCompra);
            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task EliminarAsync(Guid id)
        {
            // Borrado lógico
            const string sql = @"UPDATE dbo.Inventario SET Activo=0, ModificadoEn=SYSUTCDATETIME() WHERE Id=@Id;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        // =========== API específica IInventarioRepo ===========

        public async Task<Inventario> GetOrCreateAsync(Guid negocioId, Guid productoId)
        {
            const string sel = @"
SELECT TOP(1) * FROM dbo.Inventario
WHERE NegocioId=@NegocioId AND ProductoId=@ProductoId AND LoteId IS NULL AND Activo=1;";
            using var cn = await OpenAsync();
            // try get
            using (var cmdSel = new SqlCommand(sel, cn))
            {
                cmdSel.Parameters.AddWithValue("@NegocioId", negocioId);
                cmdSel.Parameters.AddWithValue("@ProductoId", productoId);
                using var rd = await cmdSel.ExecuteReaderAsync();
                if (await rd.ReadAsync()) return Map(rd);
            }

            // create
            var inv = new Inventario
            {
                Id = Guid.NewGuid(),
                NegocioId = negocioId,
                ProductoId = productoId,
                CantidadDisponible = 0,
                CantidadReservada = 0,
                Minimo = 0,
                Maximo = 0,
                CostoPromedio = 0,
                CostoUltimaCompra = 0
            };

            const string ins = @"
INSERT INTO dbo.Inventario
(Id,NegocioId,ProductoId,LoteId,CantidadDisponible,CantidadReservada,Minimo,Maximo,Ubicacion,
 CostoPromedio,CostoUltimaCompra,CreadoEn,Activo)
VALUES
(@Id,@NegocioId,@ProductoId,NULL,@CantDisp,@CantRes,@Min,@Max,NULL,@CostoProm,@CostoUlt,SYSUTCDATETIME(),1);";

            using (var cmdIns = new SqlCommand(ins, cn))
            {
                cmdIns.Parameters.AddWithValue("@Id", inv.Id);
                cmdIns.Parameters.AddWithValue("@NegocioId", negocioId);
                cmdIns.Parameters.AddWithValue("@ProductoId", productoId);
                cmdIns.Parameters.AddWithValue("@CantDisp", inv.CantidadDisponible);
                cmdIns.Parameters.AddWithValue("@CantRes", inv.CantidadReservada);
                cmdIns.Parameters.AddWithValue("@Min", inv.Minimo);
                cmdIns.Parameters.AddWithValue("@Max", inv.Maximo);
                cmdIns.Parameters.AddWithValue("@CostoProm", inv.CostoPromedio);
                cmdIns.Parameters.AddWithValue("@CostoUlt", inv.CostoUltimaCompra);
                await cmdIns.ExecuteNonQueryAsync();
            }
            return inv;
        }

        public async Task AddMovimientoAsync(MovimientoInventario mov)
        {
            if (mov.Id == Guid.Empty) mov.Id = Guid.NewGuid();

            using var cn = await OpenAsync();
            using var tx = cn.BeginTransaction();

            try
            {
                // 1) Insertar movimiento
                const string insMov = @"
INSERT INTO dbo.MovimientoInventario
(Id, InventarioId, NegocioId, ProductoId, VentaId, CompraId, Tipo, Cantidad, CostoUnitario, Referencia, Usuario, Fecha)
VALUES
(@Id,@InventarioId,@NegocioId,@ProductoId,@VentaId,@CompraId,@Tipo,@Cantidad,@CostoUnitario,@Referencia,@Usuario,SYSUTCDATETIME());";
                using (var cmd = new SqlCommand(insMov, cn, tx))
                {
                    cmd.Parameters.AddWithValue("@Id", mov.Id);
                    cmd.Parameters.AddWithValue("@InventarioId", mov.InventarioId);
                    cmd.Parameters.AddWithValue("@NegocioId", mov.NegocioId);
                    cmd.Parameters.AddWithValue("@ProductoId", mov.ProductoId);
                    cmd.Parameters.AddWithValue("@VentaId", (object?)mov.VentaId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompraId", (object?)mov.CompraId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tipo", (int)mov.Tipo);
                    cmd.Parameters.AddWithValue("@Cantidad", mov.Cantidad);
                    cmd.Parameters.AddWithValue("@CostoUnitario", mov.CostoUnitario);
                    cmd.Parameters.AddWithValue("@Referencia", (object?)mov.Referencia ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Usuario", (object?)mov.Usuario ?? DBNull.Value);
                    await cmd.ExecuteNonQueryAsync();
                }

                // 2) Actualizar existencias (delta)
                var delta = mov.Tipo switch
                {
                    TipoMovimiento.Entrada => mov.Cantidad,
                    TipoMovimiento.Salida => -mov.Cantidad,
                    _ => mov.Cantidad // Ajuste: puede venir + o -
                };

                const string updInv = @"
UPDATE dbo.Inventario
SET CantidadDisponible = CantidadDisponible + @Delta,
    ModificadoEn = SYSUTCDATETIME()
WHERE Id=@InventarioId AND Activo=1;";
                using (var cmd2 = new SqlCommand(updInv, cn, tx))
                {
                    cmd2.Parameters.AddWithValue("@Delta", delta);
                    cmd2.Parameters.AddWithValue("@InventarioId", mov.InventarioId);
                    await cmd2.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<IReadOnlyList<MovimientoInventario>> ListMovimientosAsync(
            Guid negocioId, Guid productoId, DateTime? desde = null, DateTime? hasta = null)
        {
            var sql = @"
SELECT * FROM dbo.MovimientoInventario
WHERE NegocioId=@NegocioId AND ProductoId=@ProductoId";
            if (desde.HasValue) sql += " AND Fecha >= @Desde";
            if (hasta.HasValue) sql += " AND Fecha <= @Hasta";
            sql += " ORDER BY Fecha DESC;";

            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@NegocioId", negocioId);
            cmd.Parameters.AddWithValue("@ProductoId", productoId);
            if (desde.HasValue) cmd.Parameters.AddWithValue("@Desde", desde.Value);
            if (hasta.HasValue) cmd.Parameters.AddWithValue("@Hasta", hasta.Value);

            var list = new List<MovimientoInventario>();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new MovimientoInventario
                {
                    Id = Get<Guid>(rd, "Id"),
                    InventarioId = Get<Guid>(rd, "InventarioId"),
                    NegocioId = Get<Guid>(rd, "NegocioId"),
                    ProductoId = Get<Guid>(rd, "ProductoId"),
                    VentaId = Get<Guid?>(rd, "VentaId"),
                    CompraId = Get<Guid?>(rd, "CompraId"),
                    Tipo = (TipoMovimiento)Get<int>(rd, "Tipo"),
                    Cantidad = Get<decimal>(rd, "Cantidad"),
                    CostoUnitario = Get<decimal>(rd, "CostoUnitario"),
                    Referencia = Get<string>(rd, "Referencia"),
                    Usuario = Get<string>(rd, "Usuario"),
                    Fecha = Get<DateTime>(rd, "Fecha")
                });
            }
            return list;
        }

        // =========== Helpers ===========

        private static Inventario Map(IDataRecord r) => new Inventario
        {
            Id = Get<Guid>(r, "Id"),
            NegocioId = Get<Guid>(r, "NegocioId"),
            ProductoId = Get<Guid>(r, "ProductoId"),
            LoteId = Get<Guid?>(r, "LoteId"),
            CantidadDisponible = Get<decimal>(r, "CantidadDisponible"),
            CantidadReservada = Get<decimal>(r, "CantidadReservada"),
            Minimo = Get<decimal>(r, "Minimo"),
            Maximo = Get<decimal>(r, "Maximo"),
            Ubicacion = Get<string>(r, "Ubicacion"),
            CostoPromedio = Get<decimal>(r, "CostoPromedio"),
            CostoUltimaCompra = Get<decimal>(r, "CostoUltimaCompra"),
            CreadoEn = Get<DateTime>(r, "CreadoEn"),
            ModificadoEn = Get<DateTime?>(r, "ModificadoEn")
        };
    }
}
