using Microsoft.Maui.Controls;
using System;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaContratos;         // INegocioRepo
using Tiendara.CapaVisual.Utils;      // ServiceResolver, SessionService

namespace Tiendara.CapaVisual.Autenticacion
{
    public partial class RegistroTiendaPage : ContentPage
    {
        private readonly INegocioRepo _repo = ServiceResolver.Get<INegocioRepo>();
        private readonly SessionService _session = ServiceResolver.Get<SessionService>();

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
                PropietarioUsuarioId = _session.UsuarioId,
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
