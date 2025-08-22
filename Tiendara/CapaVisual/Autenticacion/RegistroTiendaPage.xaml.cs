using Microsoft.Maui.Controls;
using System;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaContratos;        // INegocioRepo
using Tiendara.CapaLogica.Servicios; // ← SessionService de CapaLógica
using Tiendara.CapaVisual.Utils;
using Tiendara.CapaLogica.Servicios.Tiendara.CapaLogica.Servicios;     // ServiceResolver solo si quieres usar DI

namespace Tiendara.CapaVisual.Autenticacion
{
    public partial class RegistroTiendaPage : ContentPage
    {
        private readonly INegocioRepo _repo = ServiceResolver.Get<INegocioRepo>();
        private readonly SessionService _session = ServiceResolver.Get<SessionService>(); // ahora es de CapaLógica

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

            if (!_session.Autenticado)
            {
                await DisplayAlert("Tienda", "Primero inicia sesión.", "OK");
                return;
            }

            var n = new Negocio
            {
                PropietarioUsuarioId = _session.UsuarioId,  // toma el ID del usuario logeado
                Nombre = nombre,
                Giro = (txtGiro.Text ?? "").Trim(),
                Telefono = (txtTelefono.Text ?? "").Trim(),
                Direccion = (txtDireccion.Text ?? "").Trim(),
                Abierto = true
            };

            await _repo.AddAsync(n);
            await DisplayAlert("Tienda", "¡Tienda registrada!", "OK");
            await Navigation.PopAsync();
        }
    }
}
