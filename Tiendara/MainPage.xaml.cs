using System;
using Microsoft.Maui.Controls;

namespace Tiendara
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnInventarioClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("inventario");
        }

        private async void OnVentaClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("venta");
        }

        private async void OnCorteCajaClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("corte");
        }

        private async void OnEmpleadosClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("empleados");
        }

        private async void OnProveedoresClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("proveedores");
        }

        private async void OnCerrarSesionClicked(object sender, EventArgs e)
        {
            // Luego puedes navegar a Login o reiniciar app
            await DisplayAlert("Cerrar sesión", "Sesión cerrada correctamente", "OK");
        }
    }
}
