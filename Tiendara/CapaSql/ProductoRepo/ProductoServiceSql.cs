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
using Tiendara.CapaSql.Base;
using Tiendara.CapaSql.Conexion;

namespace Tiendara.CapaSql.ProductoRepo
{
    public class ProductoServiceSql : CrudSqlBase<Producto>, IProductoRepo
    {
        public ProductoServiceSql() : base(ConfiguracionSql.ConnectionString) { }

        // ===== CRUD base =====
        public override async Task<Producto?> ObtenerPorIdAsync(Guid id)
        {
            const string sql = "SELECT TOP(1) * FROM dbo.Producto WHERE Id=@Id AND Activo=1;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var rd = await cmd.ExecuteReaderAsync();
            return await rd.ReadAsync() ? Map(rd) : null;
        }

        public override async Task<List<Producto>> ObtenerTodosAsync()
        {
            const string sql = "SELECT * FROM dbo.Producto WHERE Activo=1 ORDER BY CreadoEn DESC;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<Producto>();
            while (await rd.ReadAsync()) list.Add(Map(rd));
            return list;
        }

        public override async Task InsertarAsync(Producto e)
        {
            if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();

            const string sql = @"
INSERT INTO dbo.Producto
(Id, SKU, CodigoBarras, Nombre, Marca, CategoriaTexto, Categoria, UnidadVenta, PrecioBase, TasaImpuesto,
 TipoMedida, EsPerecedero, FechaVencimiento, Estado, Oferta, ValorOferta, FechaInicioOferta, FechaFinOferta,
 CreadoEn, Activo)
VALUES
(@Id, @SKU, @CodigoBarras, @Nombre, @Marca, @CategoriaTexto, @Categoria, @UnidadVenta, @PrecioBase, @TasaImpuesto,
 @TipoMedida, @EsPerecedero, @FechaVencimiento, @Estado, @Oferta, @ValorOferta, @FechaInicioOferta, @FechaFinOferta,
 SYSUTCDATETIME(), 1);";

            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            AddParams(cmd, e, includeId: true);
            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task ActualizarAsync(Producto e)
        {
            const string sql = @"
UPDATE dbo.Producto SET
  SKU=@SKU, CodigoBarras=@CodigoBarras, Nombre=@Nombre, Marca=@Marca, CategoriaTexto=@CategoriaTexto,
  Categoria=@Categoria, UnidadVenta=@UnidadVenta, PrecioBase=@PrecioBase, TasaImpuesto=@TasaImpuesto,
  TipoMedida=@TipoMedida, EsPerecedero=@EsPerecedero, FechaVencimiento=@FechaVencimiento,
  Estado=@Estado, Oferta=@Oferta, ValorOferta=@ValorOferta, FechaInicioOferta=@FechaInicioOferta, FechaFinOferta=@FechaFinOferta,
  ModificadoEn=SYSUTCDATETIME()
WHERE Id=@Id;";

            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            AddParams(cmd, e, includeId: true);
            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task EliminarAsync(Guid id)
        {
            // borrado lógico
            const string sql = "UPDATE dbo.Producto SET Activo=0, ModificadoEn=SYSUTCDATETIME() WHERE Id=@Id;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        // ===== IProductoRepo =====
        public async Task<List<Producto>> BuscarPorNombreAsync(string nombre)
        {
            const string sql = "SELECT * FROM dbo.Producto WHERE Activo=1 AND Nombre LIKE @Nombre ORDER BY Nombre;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Nombre", $"%{nombre}%");
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<Producto>();
            while (await rd.ReadAsync()) list.Add(Map(rd));
            return list;
        }

        public async Task<List<Producto>> ListarPorCategoriaAsync(CategoriaProducto categoria)
        {
            const string sql = "SELECT * FROM dbo.Producto WHERE Activo=1 AND Categoria=@Categoria ORDER BY Nombre;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Categoria", (int)categoria);
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<Producto>();
            while (await rd.ReadAsync()) list.Add(Map(rd));
            return list;
        }

        // ===== helpers =====
        private static Producto Map(IDataRecord r) => new Producto
        {
            Id = Get<Guid>(r, "Id"),
            SKU = Get<string>(r, "SKU") ?? string.Empty,
            CodigoBarras = Get<string>(r, "CodigoBarras") ?? string.Empty,
            Nombre = Get<string>(r, "Nombre") ?? string.Empty,
            Marca = Get<string>(r, "Marca") ?? string.Empty,
            CategoriaTexto = Get<string>(r, "CategoriaTexto") ?? string.Empty,
            Categoria = (CategoriaProducto)Get<int>(r, "Categoria"),
            UnidadVenta = (UnidadVenta)Get<int>(r, "UnidadVenta"),
            PrecioBase = Get<decimal>(r, "PrecioBase"),
            TasaImpuesto = Get<decimal>(r, "TasaImpuesto"),
            TipoMedida = (TipoMedida)Get<int>(r, "TipoMedida"),
            EsPerecedero = Get<bool>(r, "EsPerecedero"),
            FechaVencimiento = Get<DateTime?>(r, "FechaVencimiento"),
            Estado = (EstadoProducto)Get<int>(r, "Estado"),
            Oferta = (TipoOferta)Get<int>(r, "Oferta"),
            ValorOferta = Get<decimal?>(r, "ValorOferta"),
            FechaInicioOferta = Get<DateTime?>(r, "FechaInicioOferta"),
            FechaFinOferta = Get<DateTime?>(r, "FechaFinOferta")
        };

        private static void AddParams(SqlCommand cmd, Producto e, bool includeId)
        {
            if (includeId) cmd.Parameters.AddWithValue("@Id", e.Id);
            cmd.Parameters.AddWithValue("@SKU", e.SKU);
            cmd.Parameters.AddWithValue("@CodigoBarras", e.CodigoBarras);
            cmd.Parameters.AddWithValue("@Nombre", e.Nombre);
            cmd.Parameters.AddWithValue("@Marca", e.Marca);
            cmd.Parameters.AddWithValue("@CategoriaTexto", e.CategoriaTexto);
            cmd.Parameters.AddWithValue("@Categoria", (int)e.Categoria);
            cmd.Parameters.AddWithValue("@UnidadVenta", (int)e.UnidadVenta);
            cmd.Parameters.AddWithValue("@PrecioBase", e.PrecioBase);
            cmd.Parameters.AddWithValue("@TasaImpuesto", e.TasaImpuesto);
            cmd.Parameters.AddWithValue("@TipoMedida", (int)e.TipoMedida);
            cmd.Parameters.AddWithValue("@EsPerecedero", e.EsPerecedero);
            cmd.Parameters.AddWithValue("@FechaVencimiento", (object?)e.FechaVencimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", (int)e.Estado);
            cmd.Parameters.AddWithValue("@Oferta", (int)e.Oferta);
            cmd.Parameters.AddWithValue("@ValorOferta", (object?)e.ValorOferta ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaInicioOferta", (object?)e.FechaInicioOferta ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaFinOferta", (object?)e.FechaFinOferta ?? DBNull.Value);
        }
    }
}
