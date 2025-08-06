// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [qué hace esta clase]
/// <summary>
/// Representa un producto en el sistema.
/// </summary>
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
    public class Producto
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public int Stock { get; set; }
        public decimal Precio { get; set; }
        public string Tipo { get; set; } // Bebida, Comida,Etc.
        public string CodigoDeBarras { get; set; }
        public string Proveedor { get; set; }
    }
}