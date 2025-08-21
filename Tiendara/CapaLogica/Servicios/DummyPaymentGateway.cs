// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------


using System.Threading;
using System.Threading.Tasks;
using Tiendara.CapaContratos;

namespace Tiendara.CapaLogica.Servicios
{
    public sealed class DummyPaymentGateway : IPaymentGateway
    {
        public Task<PaymentResult> CobrarAsync(decimal monto, string concepto, CancellationToken ct = default)
            => Task.FromResult(new PaymentResult(true, $"AUTH-{DateTime.UtcNow.Ticks}", null));
    }
}
