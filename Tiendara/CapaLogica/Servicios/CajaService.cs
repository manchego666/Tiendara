using System;
using System.Linq;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaDatos.Repos;
using static Tiendara.CapaLogica.Mathx;

namespace Tiendara.CapaLogica.Servicios;

public sealed class CajaService : ICajaService
{
    private readonly ICajaRepo _repoCaja;
    private readonly IVentaRepo _repoVenta;

    public CajaService(ICajaRepo cajaRepo, IVentaRepo ventaRepo)
    {
        _repoCaja = cajaRepo;
        _repoVenta = ventaRepo;
    }

    public async Task<Guid> RegistrarRetiro(Guid negocioId, decimal monto, string concepto,
                                            string? usuario = null, MedioPago medio = MedioPago.Efectivo)
    {
        if (monto <= 0) throw new ArgumentException("Monto debe ser > 0");
        var mov = new MovimientoCaja
        {
            NegocioId = negocioId,
            Tipo = TipoMovimientoCaja.Retiro,
            Monto = R2(monto),
            Medio = medio,
            Concepto = concepto,
            Usuario = usuario,
            Fecha = DateTime.Now
        };
        await _repoCaja.AddMovimientoAsync(mov);
        return mov.Id;
    }

    public async Task<CorteCaja> GenerarCorte(Guid negocioId, DateTime desde, DateTime hasta)
    {
        var movs = await _repoCaja.ListMovimientosAsync(negocioId, desde, hasta);
        var ingresos = movs.Where(m => m.Tipo == TipoMovimientoCaja.IngresoVenta).Sum(m => m.Monto);
        var retiros = movs.Where(m => m.Tipo == TipoMovimientoCaja.Retiro).Sum(m => m.Monto);

        var ventas = await _repoVenta.ListByFechaAsync(negocioId, desde, hasta);
        return new CorteCaja
        {
            NegocioId = negocioId,
            Desde = desde,
            Hasta = hasta,
            Ingresos = R2(ingresos),
            Retiros = R2(retiros),
            Ventas = ventas.Count
        };
    }
}
