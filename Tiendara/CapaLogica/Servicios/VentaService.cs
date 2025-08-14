using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaDatos.Repos;
using static Tiendara.CapaLogica.Mathx;

namespace Tiendara.CapaLogica.Servicios;

public sealed class VentaService : IVentaService
{
    private readonly IVentaRepo _ventaRepo;
    private readonly IInventarioRepo _invRepo;
    private readonly IInventarioService _invService;
    private readonly ICajaRepo _cajaRepo;

    public VentaService(IVentaRepo ventaRepo, IInventarioRepo invRepo, IInventarioService invService, ICajaRepo cajaRepo)
    {
        _ventaRepo = ventaRepo;
        _invRepo = invRepo;
        _invService = invService;
        _cajaRepo = cajaRepo;
    }

    public async Task<Guid> RegistrarVentaAsync(IVendedor vendedor, SolicitudVenta s, CancellationToken ct = default)
    {
        if (vendedor is null) throw new ArgumentNullException(nameof(vendedor));
        if (!vendedor.PuedeVender) throw new InvalidOperationException("Usuario no autorizado para vender.");
        if (s.NegocioId is null) throw new ArgumentException("NegocioId requerido.");
        if (s.Lineas == null || s.Lineas.Count == 0) throw new ArgumentException("La venta no tiene líneas.");

        var negocioId = s.NegocioId.Value;

        // Calcula totales
        decimal subtotal = 0, impuestos = 0, total = 0;
        var venta = new Venta
        {
            NegocioId = negocioId,
            VendedorUsuarioId = vendedor.UsuarioId,
            Referencia = s.Referencia,
            MedioPago = s.MedioPago,
            Estado = EstadoVenta.Finalizada
        };

        foreach (var l in s.Lineas)
        {
            // Tomar el precio unitario y tasa (si vienen 0/null, podrías consultar catálogo aquí)
            var precio = l.PrecioUnitario;
            if (precio <= 0) throw new ArgumentException("Precio unitario inválido en línea.");
            var tasa = l.TasaImpuesto ?? 0m;

            var importe = R2(precio * l.Cantidad);
            var imp = R2(importe * tasa);

            subtotal = R2(subtotal + importe);
            impuestos = R2(impuestos + imp);

            venta.Lineas.Add(new VentaLinea
            {
                ItemId = l.ItemId,
                Nombre = l.Descripcion ?? "Item", // o consulta catálogo si quieres
                Unidad = UnidadVenta.Pza,        // si quieres, pasa la unidad desde UI
                Cantidad = l.Cantidad,
                PrecioUnitario = precio,
                TasaImpuesto = tasa,
                Importe = importe
            });
        }

        total = R2(subtotal + impuestos);
        venta.Subtotal = subtotal;
        venta.Impuestos = impuestos;
        venta.Total = total;

        // Descontar inventario (solo para productos físicos; aquí asumimos que todas las líneas son producto;
        // si mezclas servicios, márcalos desde UI y sáltalos)
        foreach (var l in s.Lineas)
        {
            await _invService.RegistrarSalida(negocioId, l.ItemId, l.Cantidad, ventaId: venta.Id,
                                              referencia: s.Referencia, usuario: vendedor.UsuarioId.ToString());
        }

        // Pago y cambio
        venta.PagoRecibido = s.PagoRecibido > 0 ? R2(s.PagoRecibido) : total;
        venta.Cambio = R2((venta.PagoRecibido ?? total) - total);

        // Persistir venta
        await _ventaRepo.AddAsync(venta);

        // Movimiento de caja (IngresoVenta)
        var mov = new MovimientoCaja
        {
            NegocioId = negocioId,
            Tipo = TipoMovimientoCaja.IngresoVenta,
            Monto = venta.PagoRecibido ?? total,
            Medio = s.MedioPago,
            VentaId = venta.Id,
            Concepto = s.Referencia ?? $"Venta {venta.Id}",
            Usuario = vendedor.UsuarioId.ToString(),
            Fecha = DateTime.Now
        };
        await _cajaRepo.AddMovimientoAsync(mov);

        return venta.Id;
    }
}
