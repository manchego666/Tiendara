// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaLogica.Servicios
{
    public sealed class VentaService : IVentaService
    {
        private readonly IProductoRepo _productoRepo;
        private readonly IVentaRepo _ventaRepo;
        private readonly IInventarioRepo _invRepo;
        private readonly ICajaRepo _cajaRepo;
        private readonly IPaymentGateway? _payment; // opcional
        private readonly Microsoft.Extensions.Logging.ILogger<VentaService>? _log;

        public VentaService(
            IProductoRepo productoRepo,
            IVentaRepo ventaRepo,
            IInventarioRepo invRepo,
            ICajaRepo cajaRepo,
            IPaymentGateway? payment = null,
            Microsoft.Extensions.Logging.ILogger<VentaService>? log = null)
        {
            _productoRepo = productoRepo;
            _ventaRepo = ventaRepo;
            _invRepo = invRepo;
            _cajaRepo = cajaRepo;
            _payment = payment;
            _log = log;
        }

        public async Task<Guid> RegistrarVentaAsync(IVendedor vendedor, SolicitudVenta s, CancellationToken ct = default)
        {
            if (vendedor is null) throw new ArgumentNullException(nameof(vendedor));
            if (!vendedor.PuedeVender) throw new InvalidOperationException("El vendedor no tiene permisos para vender.");
            if (s is null) throw new ArgumentNullException(nameof(s));
            if (s.Lineas is null || s.Lineas.Count == 0) throw new InvalidOperationException("La venta no tiene líneas.");

            var venta = new Venta
            {
                NegocioId = s.NegocioId,
                VendedorUsuarioId = vendedor.UsuarioId,
                MedioPago = s.MedioPago
            };

            decimal subtotal = 0m;
            decimal impuestos = 0m;

            foreach (var lin in s.Lineas)
            {
                var prod = await _productoRepo.ObtenerPorIdAsync(lin.ItemId);
                if (prod is null) throw new InvalidOperationException($"Producto {lin.ItemId} no existe.");
                if (!prod.EstaDisponibleParaVenta()) throw new InvalidOperationException($"Producto {prod.Nombre} no está disponible.");
                if (lin.Cantidad <= 0) throw new InvalidOperationException($"Cantidad inválida para {prod.Nombre}.");

                var inv = await _invRepo.GetOrCreateAsync(s.NegocioId, prod.Id);
                if (inv.DisponibleParaVenta < lin.Cantidad)
                    throw new InvalidOperationException($"Stock insuficiente de '{prod.Nombre}'. Disponible: {inv.DisponibleParaVenta}, solicitado: {lin.Cantidad}");

                var precio = prod.ObtenerPrecioFinal();
                var importe = Mathx.R2(precio * lin.Cantidad);
                var imp = Mathx.R2(importe * prod.TasaImpuesto);

                venta.Lineas.Add(new VentaLinea
                {
                    ItemId = prod.Id,
                    Nombre = prod.Nombre,
                    Unidad = prod.UnidadVenta,
                    Cantidad = lin.Cantidad,
                    PrecioUnitario = precio,
                    TasaImpuesto = prod.TasaImpuesto,
                    Importe = importe
                });

                subtotal += importe;
                impuestos += imp;
            }

            venta.Subtotal = Mathx.R2(subtotal);
            venta.Impuestos = Mathx.R2(impuestos);
            venta.Total = Mathx.R2(venta.Subtotal + venta.Impuestos);

            // Pago
            switch (s.MedioPago)
            {
                case MedioPago.Efectivo:
                    if (s.PagoRecibido > 0)
                    {
                        if (s.PagoRecibido < venta.Total)
                            throw new InvalidOperationException($"Efectivo insuficiente. Total: {venta.Total}, recibido: {s.PagoRecibido}");
                        venta.PagoRecibido = s.PagoRecibido;
                        venta.Cambio = Mathx.R2(s.PagoRecibido - venta.Total);
                    }
                    else
                    {
                        venta.PagoRecibido = null;
                        venta.Cambio = null;
                    }
                    break;

                case MedioPago.Tarjeta:
                case MedioPago.Transferencia:
                    if (_payment is not null)
                    {
                        var r = await _payment.CobrarAsync(venta.Total, $"Venta {venta.Id}", ct);
                        if (!r.Ok)
                        {
                            _log?.LogWarning("Pago rechazado para Negocio {NegocioId}, Vendedor {VendedorId}: {Error}",
                                venta.NegocioId, venta.VendedorUsuarioId, r.Error);
                            throw new InvalidOperationException($"Pago rechazado: {r.Error ?? "Error desconocido"}");
                        }
                        venta.Referencia = string.IsNullOrWhiteSpace(s.PagoReferencia) ? r.AuthorizationCode : s.PagoReferencia;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(s.PagoReferencia))
                            throw new InvalidOperationException("PagoReferencia es obligatoria para tarjeta/transferencia si no hay gateway.");
                        venta.Referencia = s.PagoReferencia;
                    }
                    venta.PagoRecibido = null;
                    venta.Cambio = null;
                    break;

                case MedioPago.Mixto:
                    if (string.IsNullOrWhiteSpace(s.PagoReferencia))
                        throw new InvalidOperationException("PagoReferencia requerida para pago mixto.");
                    venta.Referencia = s.PagoReferencia;
                    venta.PagoRecibido = null;
                    venta.Cambio = null;
                    break;

                default:
                    throw new InvalidOperationException("Medio de pago no soportado.");
            }

            await _ventaRepo.AddAsync(venta);

            foreach (var lin in venta.Lineas)
            {
                var inv = await _invRepo.GetOrCreateAsync(venta.NegocioId, lin.ItemId);
                var mov = new MovimientoInventario
                {
                    InventarioId = inv.Id,
                    NegocioId = venta.NegocioId,
                    ProductoId = lin.ItemId,
                    VentaId = venta.Id,
                    Tipo = TipoMovimiento.Salida,
                    Cantidad = lin.Cantidad,
                    CostoUnitario = inv.CostoPromedio,
                    Referencia = $"Venta {venta.Id}",
                    Usuario = vendedor.UsuarioId.ToString()
                };
                await _invRepo.AddMovimientoAsync(mov);
            }

            var movCaja = new MovimientoCaja
            {
                NegocioId = venta.NegocioId,
                Tipo = TipoMovimientoCaja.IngresoVenta,
                Monto = venta.Total,
                Medio = venta.MedioPago,
                VentaId = venta.Id,
                Concepto = $"Venta {venta.Id}",
                Usuario = vendedor.UsuarioId.ToString()
            };
            await _cajaRepo.AddMovimientoAsync(movCaja);

            _log?.LogInformation("Venta {VentaId} creada para Negocio {NegocioId}. Total: {Total}, Medio: {Medio}",
                venta.Id, venta.NegocioId, venta.Total, venta.MedioPago);

            return venta.Id;
        }
    }
}
