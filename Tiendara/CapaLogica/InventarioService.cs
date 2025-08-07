// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Servicio para gestionar productos en inventario.
// Fecha: 2025-08-06
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace Tiendara.CapaLogica
{
    public static class InventarioService
    {
        private static ObservableCollection<Producto> productos = new();


        public static void AgregarProducto(Producto producto)
        {
            productos.Add(producto);
        }

        public static ObservableCollection<Producto> ObtenerTodos() => productos;

        public static Producto BuscarPorCodigo(string codigo) =>
            productos.FirstOrDefault(p => p.CodigoDeBarras == codigo);

        public static bool EliminarProducto(int id)
        {
            var producto = productos.FirstOrDefault(p => p.ID == id);
            if (producto != null)
            {
                productos.Remove(producto);
                return true;
            }
            return false;
        }

        public static void EditarProducto(Producto actualizado)
        {
            for (int i = 0; i < productos.Count; i++)
            {
                if (productos[i].ID == actualizado.ID)
                {
                    productos[i] = actualizado;
                    break;
                }
            }
        }


        public static Producto ObtenerProductoPorId(int id)
        {
            return productos.FirstOrDefault(p => p.ID == id);
        }


    }
}