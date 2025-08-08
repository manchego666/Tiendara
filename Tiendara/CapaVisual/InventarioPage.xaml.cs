using System;
using Microsoft.Maui.Controls;
using Tiendara.CapaLogica;

namespace Tiendara
{
    public partial class InventarioPage : ContentPage
    {
        List<Producto> productos = new();
        Producto productoEnEdicion = null;

        public InventarioPage()
        {
            InitializeComponent();
            listaProductos.ItemsSource = InventarioService.ObtenerTodos();
        }

        private void OnGuardarClicked(object sender, EventArgs e)
        {
            if (productoEnEdicion != null)
            {
                // Editar existente
                productoEnEdicion.Nombre = txtNombre.Text;
                productoEnEdicion.Marca = txtMarca.Text;
                productoEnEdicion.Tipo = txtTipo.Text;
                productoEnEdicion.CodigoDeBarras = txtCodigo.Text;
                productoEnEdicion.Proveedor = txtProveedor.Text;
                productoEnEdicion.PrecioCompra = decimal.Parse(txtPrecioCompra.Text);
                productoEnEdicion.PrecioVenta = decimal.Parse(txtPrecioVenta.Text);
                productoEnEdicion.Stock = int.Parse(txtStock.Text);
                productoEnEdicion.Unidad = txtUnidad.Text;
                productoEnEdicion.ContenidoNeto = txtContenido.Text;

                productoEnEdicion = null; // ya no estamos editando
            }
            else
            {
                // Nuevo producto
                var nuevoProducto = new Producto
                {
                    ID = productos.Count + 1,
                    Nombre = txtNombre.Text,
                    Marca = txtMarca.Text,
                    Tipo = txtTipo.Text,
                    CodigoDeBarras = txtCodigo.Text,
                    Proveedor = txtProveedor.Text,
                    PrecioCompra = decimal.Parse(txtPrecioCompra.Text),
                    PrecioVenta = decimal.Parse(txtPrecioVenta.Text),
                    Stock = int.Parse(txtStock.Text),
                    Unidad = txtUnidad.Text,
                    ContenidoNeto = txtContenido.Text
                };
                productos.Add(nuevoProducto);
            }

            listaProductos.ItemsSource = null;
            listaProductos.ItemsSource = productos;

            // Limpiar formulario
            txtNombre.Text = txtMarca.Text = txtTipo.Text = txtCodigo.Text = txtProveedor.Text = "";
            txtPrecioCompra.Text = txtPrecioVenta.Text = txtStock.Text = "";
            txtUnidad.Text = txtContenido.Text = "";
        }


        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtMarca.Text = "";
            txtTipo.Text = "";
            txtCodigo.Text = "";
            txtProveedor.Text = "";
            txtPrecioCompra.Text = "";
            txtPrecioVenta.Text = "";
            txtStock.Text = "";
            txtUnidad.Text = "";
            txtContenido.Text = "";
        }

        private void OnEliminarProducto(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is int id)
            {
                var producto = productos.FirstOrDefault(p => p.ID == id);
                if (producto != null)
                {
                    productos.Remove(producto);
                    listaProductos.ItemsSource = null;
                    listaProductos.ItemsSource = productos;
                }
            }
            else if (sender is Button boton && boton.CommandParameter is int idBoton)
            {
                var producto = productos.FirstOrDefault(p => p.ID == idBoton);
                if (producto != null)
                {
                    productos.Remove(producto);
                    listaProductos.ItemsSource = null;
                    listaProductos.ItemsSource = productos;
                }
            }
        }


        private void OnEditarProducto(object sender, EventArgs e)
        {
            int? id = null;

            if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is int idSwipe)
                id = idSwipe;
            else if (sender is Button boton && boton.CommandParameter is int idBtn)
                id = idBtn;

            if (id.HasValue)
            {
                var producto = productos.FirstOrDefault(p => p.ID == id.Value);
                if (producto != null)
                {
                    // Cargar datos al formulario
                    txtNombre.Text = producto.Nombre;
                    txtMarca.Text = producto.Marca;
                    txtTipo.Text = producto.Tipo;
                    txtCodigo.Text = producto.CodigoDeBarras;
                    txtProveedor.Text = producto.Proveedor;
                    txtPrecioCompra.Text = producto.PrecioCompra.ToString();
                    txtPrecioVenta.Text = producto.PrecioVenta.ToString();
                    txtStock.Text = producto.Stock.ToString();
                    txtUnidad.Text = producto.Unidad;
                    txtContenido.Text = producto.ContenidoNeto;

                    // Guardar ID en variable para saber que estamos editando
                    productoEnEdicion = producto;
                }
            }
        }

    }
}
