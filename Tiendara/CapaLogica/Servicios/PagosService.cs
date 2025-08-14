using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaLogica.Servicios
{
    /// <summary>
    /// Servicio de cobro para una venta: valida importes por método de pago,
    /// calcula cambio y permite pagos combinados (efectivo/tarjeta/transfer).
    /// Es agnóstico de pasarelas (Stripe, MercadoPago): tú las llamas fuera.
    /// </summary>
    public class PagosService
    {
        public record ResultadoCobro(
            decimal TotalVenta,
            decimal EntregadoEfectivo,
            decimal EntregadoTarjeta,
            decimal EntregadoTransferencia,
            decimal EntregadoTotal,
            decimal Cambio,
            decimal Pendiente);

        /// <summary>
        /// Procesa el cobro de una venta con múltiples métodos.
        /// Si no se permite pendiente y falta dinero, lanza excepción.
        /// </summary>
        /// <param name="venta">Venta (usa Total/Subtotal/Impuestos ya calculados).</param>
        /// <param name="efectivo">Monto en efectivo.</param>
        /// <param name="tarjeta">Monto por tarjeta.</param>
        /// <param name="transferencia">Monto por transferencia.</param>
        /// <param name="permitirPendiente">Si true, permite que el cliente quede a deber.</param>
        public ResultadoCobro Cobrar(
            Venta venta, decimal efectivo, decimal tarjeta = 0m, decimal transferencia = 0m,
            bool permitirPendiente = false)
        {
            if (venta == null) throw new ArgumentNullException(nameof(venta));
            if (efectivo < 0 || tarjeta < 0 || transferencia < 0)
                throw new ArgumentOutOfRangeException("Ningún método de pago puede ser negativo.");

            var totalVenta = Red2(venta.Total);
            var entregadoTotal = Red2(efectivo + tarjeta + transferencia);

            if (!permitirPendiente && entregadoTotal < totalVenta)
                throw new InvalidOperationException($"Pago insuficiente. Total: {totalVenta}, entregado: {entregadoTotal}");

            // Cambio sólo se devuelve sobre efectivo (regla habitual de caja):
            // si hay sobrepago, preferimos descontar del efectivo primero
            var exceso = entregadoTotal - totalVenta;
            var cambio = exceso > 0 ? Math.Min(efectivo, exceso) : 0m;

            var pendiente = Math.Max(0m, totalVenta - entregadoTotal);
            return new ResultadoCobro(
                TotalVenta: totalVenta,
                EntregadoEfectivo: Red2(efectivo),
                EntregadoTarjeta: Red2(tarjeta),
                EntregadoTransferencia: Red2(transferencia),
                EntregadoTotal: Red2(entregadoTotal),
                Cambio: Red2(cambio),
                Pendiente: Red2(pendiente)
            );
        }

        private static decimal Red2(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);
    }
}