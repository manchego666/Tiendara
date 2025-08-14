using System;
using System.Linq;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaDatos.Repos;
using static Tiendara.CapaLogica.Mathx;

namespace Tiendara.CapaLogica.Servicios
{
    public sealed class InventarioService : IInventarioService
    {
        private readonly IInventarioRepo _repo;
        public InventarioService(IInventarioRepo repo) => _repo = repo;

        public async Task RegistrarEntrada(Guid negocioId, Guid productoId, decimal cantidad, decimal costoUnitario,
                                           string? referencia = null, string? usuario = null)
        {
            if (cantidad <= 0) throw new ArgumentOutOfRangeException(nameof(cantidad));
            if (costoUnitario < 0) throw new ArgumentOutOfRangeException(nameof(costoUnitario));

            var inv = await _repo.GetOrCreateAsync(negocioId, productoId);

            // Moving average
            var totalActual = inv.CantidadDisponible * inv.CostoPromedio;
            var totalEntrada = cantidad * costoUnitario;
            var nuevaCantidad = inv.CantidadDisponible + cantidad;
            var nuevoCostoProm = nuevaCantidad > 0 ? (totalActual + totalEntrada) / nuevaCantidad : inv.CostoPromedio;

            inv.CantidadDisponible = R2(nuevaCantidad);
            inv.CostoPromedio = R4(nuevoCostoProm);
            inv.CostoUltimaCompra = R4(costoUnitario);
            inv.ModificadoEn = DateTime.Now;

            await _repo.UpdateAsync(inv);

            var mov = new MovimientoInventario
            {
                InventarioId = inv.Id,
                NegocioId = negocioId,
                ProductoId = productoId,
                Tipo = TipoMovimiento.Entrada,
                Cantidad = R2(cantidad),
                CostoUnitario = R4(costoUnitario),
                Referencia = referencia,
                Usuario = usuario,
                Fecha = DateTime.Now
            };
            await _repo.AddMovimientoAsync(mov);
        }

        public async Task RegistrarSalida(Guid negocioId, Guid productoId, decimal cantidad,
                                          Guid? ventaId = null, string? referencia = null, string? usuario = null)
        {
            if (cantidad <= 0) throw new ArgumentOutOfRangeException(nameof(cantidad));
            var inv = await _repo.GetOrCreateAsync(negocioId, productoId);
            if (inv.CantidadDisponible < cantidad)
                throw new InvalidOperationException("Stock insuficiente.");

            inv.CantidadDisponible = R2(inv.CantidadDisponible - cantidad);
            inv.ModificadoEn = DateTime.Now;
            await _repo.UpdateAsync(inv);

            var mov = new MovimientoInventario
            {
                InventarioId = inv.Id,
                NegocioId = negocioId,
                ProductoId = productoId,
                VentaId = ventaId,
                Tipo = TipoMovimiento.Salida,
                Cantidad = R2(cantidad),
                CostoUnitario = inv.CostoPromedio,
                Referencia = referencia,
                Usuario = usuario,
                Fecha = DateTime.Now
            };
            await _repo.AddMovimientoAsync(mov);
        }

        public async Task Ajustar(Guid negocioId, Guid productoId, decimal ajusteCantidad, string motivo, string? usuario = null)
        {
            if (ajusteCantidad == 0) return;

            var inv = await _repo.GetOrCreateAsync(negocioId, productoId);
            var nueva = inv.CantidadDisponible + ajusteCantidad;
            if (nueva < 0) throw new InvalidOperationException("Ajuste dejaría stock negativo.");

            inv.CantidadDisponible = R2(nueva);
            inv.ModificadoEn = DateTime.Now;
            await _repo.UpdateAsync(inv);

            var mov = new MovimientoInventario
            {
                InventarioId = inv.Id,
                NegocioId = negocioId,
                ProductoId = productoId,
                Tipo = TipoMovimiento.Ajuste,
                Cantidad = R2(ajusteCantidad),
                CostoUnitario = inv.CostoPromedio,
                Referencia = motivo,
                Usuario = usuario,
                Fecha = DateTime.Now
            };
            await _repo.AddMovimientoAsync(mov);
        }
    }
}
