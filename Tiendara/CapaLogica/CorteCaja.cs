// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Explica qué hace esta clase]
// Fecha: [2025-08-05]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiendara.CapaLogica
{
    internal class CorteCaja
    {
        public DateTime Fecha { get; set; }
        public decimal DineroEnCaja { get; set; }
        public decimal Retiro { get; set; }
        public List<Venta> Ventas { get; set; } = new List<Venta>();
    }
}