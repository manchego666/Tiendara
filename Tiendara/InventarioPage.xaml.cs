using System;
using Microsoft.Maui.Controls;
using Tiendara.CapaLogica;

namespace Tiendara
{
    public partial class InventarioPage : ContentPage
    {
        public InventarioPage()
        {
            InitializeComponent();
            listaProductos.ItemsSource = InventarioService.ObtenerTodos();
        }

        private void OnGuardarClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                DisplayAlert("Error", "El nombre del producto es obligatorio.", "OK");
                return;
            }

            Producto nuevo = new()
            {
                ID = InventarioService.ObtenerTodos().Count + 1,
                Nombre = txtNombre.Text,
                Marca = txtMarca.Text,
                Tipo = txtTipo.Text,
                CodigoDeBarras = txtCodigo.Text,
                Proveedor = txtProveedor.Text,
                PrecioCompra = decimal.TryParse(txtPrecioCompra.Text, out decimal pc) ? pc : 0,
                PrecioVenta = decimal.TryParse(txtPrecioVenta.Text, out decimal pv) ? pv : 0,
                Stock = int.TryParse(txtStock.Text, out int stk) ? stk : 0,
                Unidad = txtUnidad.Text,
                ContenidoNeto = txtContenido.Text,
                FechaIngreso = DateTime.Now
            };

            InventarioService.AgregarProducto(nuevo);

            // Refrescar la lista
            listaProductos.ItemsSource = null;
            listaProductos.ItemsSource = InventarioService.ObtenerTodos();

            LimpiarCampos();
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
    }
}
