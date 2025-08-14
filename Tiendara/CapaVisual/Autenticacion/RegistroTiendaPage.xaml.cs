using Microsoft.Maui.Controls;
using System;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaDatos.Repos;
using Tiendara.CapaLogica.Servicios;

namespace Tiendara.CapaVisual.Autenticacion
{
    public partial class RegistroTiendaPage : ContentPage
    {
        private readonly INegocioRepo _repo = new NegocioRepo();

        public RegistroTiendaPage()
        {
            InitializeComponent();
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            var nombre = (txtNombre.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                await DisplayAlert("Tienda", "Escribe el nombre de tu tienda.", "OK");
                return;
            }

            if (!SesionActual.Autenticado)
            {
                await DisplayAlert("Tienda", "Primero inicia sesión.", "OK");
                return;
            }

            var n = new Negocio
            {
                PropietarioUsuarioId = SesionActual.UsuarioId,
                Nombre = nombre,
                Giro = (txtGiro.Text ?? "").Trim(),
                Telefono = (txtTelefono.Text ?? "").Trim(),
                Direccion = (txtDireccion.Text ?? "").Trim(),
                Abierto = true
            };

            await _repo.AddAsync(n);
            await DisplayAlert("Tienda", "¡Tienda registrada!", "OK");

            // después puedes ir a PerfilNegocioPage o Home
            await Navigation.PopAsync(); // cierra registro tienda
        }
    }
}
