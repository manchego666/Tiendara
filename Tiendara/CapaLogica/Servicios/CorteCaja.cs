using System;

namespace Tiendara.CapaLogica.Servicios
{
    public sealed class CorteCaja
    {
        public Guid NegocioId { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public decimal Ingresos { get; set; }
        public decimal Retiros { get; set; }
        public int Ventas { get; set; }
        public decimal Neto => Tiendara.CapaLogica.Mathx.R2(Ingresos - Retiros);
    }
}
