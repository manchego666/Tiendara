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
        public int Stock { get; set; } // Cantidad actual
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public string Tipo { get; set; } // Bebida, Comida, etc.
        public string CodigoDeBarras { get; set; }
        public string Proveedor { get; set; }
        public DateTime FechaIngreso { get; set; }

        // Nuevos campos
        public string Unidad { get; set; } // pieza, litro, caja, etc.
        public string ContenidoNeto { get; set; } // gramos, kg, etc.
        public string ImagenRuta { get; set; } // Ruta local o base64 (futuro)
        public decimal Precio { get; internal set; }
    }

}
