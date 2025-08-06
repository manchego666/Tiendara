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
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public Empleado Cajero { get; set; }
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public decimal Total => Productos.Sum(p => p.Precio);
    }
}