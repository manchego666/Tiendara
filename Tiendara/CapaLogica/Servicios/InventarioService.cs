// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaLogica.Servicios
{
    public sealed class InventarioService : IInventarioService
    {
        private readonly IInventarioRepo _invRepo;
        private readonly Microsoft.Extensions.Logging.ILogger<InventarioService>? _log;

        public InventarioService(IInventarioRepo invRepo,
            Microsoft.Extensions.Logging.ILogger<InventarioService>? log = null)
        {
            _invRepo = invRepo;
            _log = log;
        }

        public async Task RegistrarEntrada(
            Guid negocioId, Guid productoId, decimal cantidad, decimal costoUnitario,
            string? referencia = null, string? usuario = null)
        {
            if (cantidad <= 0) throw new ArgumentOutOfRangeException(nameof(cantidad));
            if (costoUnitario < 0) throw new ArgumentOutOfRangeException(nameof(costoUnitario));

            var inv = await _invRepo.GetOrCreateAsync(negocioId, productoId);
            var mov = new MovimientoInventario
            {
                InventarioId = inv.Id,
                NegocioId = negocioId,
                ProductoId = productoId,
                Tipo = TipoMovimiento.Entrada,
                Cantidad = cantidad,
                CostoUnitario = costoUnitario,
                Referencia = referencia,
                Usuario = usuario
            };
            await _invRepo.AddMovimientoAsync(mov);

            _log?.LogInformation("Entrada inventario Negocio {NegocioId}, Producto {ProductoId}, Cant {Cant}, Costo {Costo}",
                negocioId, productoId, cantidad, costoUnitario);
        }

        public async Task RegistrarSalida(
            Guid negocioId, Guid productoId, decimal cantidad,
            Guid? ventaId = null, string? referencia = null, string? usuario = null)
        {
            if (cantidad <= 0) throw new ArgumentOutOfRangeException(nameof(cantidad));

            var inv = await _invRepo.GetOrCreateAsync(negocioId, productoId);
            if (inv.DisponibleParaVenta < cantidad)
                throw new InvalidOperationException($"No hay suficiente inventario. Disponible: {inv.DisponibleParaVenta}, solicitado: {cantidad}");

            var mov = new MovimientoInventario
            {
                InventarioId = inv.Id,
                NegocioId = negocioId,
                ProductoId = productoId,
                Tipo = TipoMovimiento.Salida,
                Cantidad = cantidad,
                CostoUnitario = inv.CostoPromedio,
                VentaId = ventaId,
                Referencia = referencia,
                Usuario = usuario
            };
            await _invRepo.AddMovimientoAsync(mov);

            _log?.LogInformation("Salida inventario Negocio {NegocioId}, Producto {ProductoId}, Cant {Cant}, Venta {VentaId}",
                negocioId, productoId, cantidad, ventaId);
        }

        public async Task Ajustar(Guid negocioId, Guid productoId, decimal ajusteCantidad, string motivo, string? usuario = null)
        {
            if (ajusteCantidad == 0) return;

            var inv = await _invRepo.GetOrCreateAsync(negocioId, productoId);
            if (inv.DisponibleParaVenta + ajusteCantidad < 0)
                throw new InvalidOperationException($"El ajuste dejaría inventario negativo. Disponible: {inv.DisponibleParaVenta}, ajuste: {ajusteCantidad}");

            var mov = new MovimientoInventario
            {
                InventarioId = inv.Id,
                NegocioId = negocioId,
                ProductoId = productoId,
                Tipo = TipoMovimiento.Ajuste,
                Cantidad = ajusteCantidad,
                CostoUnitario = inv.CostoPromedio,
                Referencia = motivo,
                Usuario = usuario
            };
            await _invRepo.AddMovimientoAsync(mov);

            _log?.LogWarning("Ajuste inventario Negocio {NegocioId}, Producto {ProductoId}, Ajuste {Ajuste}, Motivo {Motivo}",
                negocioId, productoId, ajusteCantidad, motivo);
        }
    }
}
