// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaLogica.Servicios
{
    public sealed class CajaService : ICajaService
    {
        private readonly ICajaRepo _cajaRepo;
        private readonly IVentaRepo _ventaRepo;

        public CajaService(ICajaRepo cajaRepo, IVentaRepo ventaRepo)
        {
            _cajaRepo = cajaRepo;
            _ventaRepo = ventaRepo;
        }

        public async Task<Guid> RegistrarRetiro(Guid negocioId, decimal monto, string concepto, string? usuario = null, MedioPago medio = MedioPago.Efectivo)
        {
            if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto));

            var m = new MovimientoCaja
            {
                NegocioId = negocioId,
                Tipo = TipoMovimientoCaja.Retiro,
                Monto = monto,
                Medio = medio,
                Concepto = concepto,
                Usuario = usuario
            };
            await _cajaRepo.AddMovimientoAsync(m);
            return m.Id;
        }

        public async Task<CorteCaja> GenerarCorte(Guid negocioId, DateTime desde, DateTime hasta)
        {
            // Normalizamos a UTC asumiento que ya vienen en UTC; si no, conviértelos
            var movs = await _cajaRepo.ListMovimientosAsync(negocioId, desde, hasta);
            var ventas = await _ventaRepo.ListByFechaAsync(negocioId, desde, hasta);

            var totalVentas = ventas.Sum(v => v.Total);
            var totalRetiros = movs.Where(m => m.Tipo == TipoMovimientoCaja.Retiro).Sum(m => m.Monto);

            // saldo inicial/final los puedes derivar con reglas propias; aquí simple:
            var saldoInicial = 0m; // si llevas saldo continuo, podrías calcularlo
            var saldoFinal = saldoInicial + totalVentas - totalRetiros;

            return new CorteCaja
            {
                NegocioId = negocioId,
                Desde = desde,
                Hasta = hasta,
                SaldoInicial = Math.Round(saldoInicial, 2, MidpointRounding.AwayFromZero),
                TotalVentas = Math.Round(totalVentas, 2, MidpointRounding.AwayFromZero),
                TotalRetiros = Math.Round(totalRetiros, 2, MidpointRounding.AwayFromZero),
                SaldoFinal = Math.Round(saldoFinal, 2, MidpointRounding.AwayFromZero),
                Movimientos = movs.Count
            };
        }
    }
}
