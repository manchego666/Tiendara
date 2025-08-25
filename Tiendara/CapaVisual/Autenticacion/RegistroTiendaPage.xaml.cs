using Microsoft.Maui.Controls;
using System;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaContratos;        // INegocioRepo
using Tiendara.CapaLogica.Servicios; // SessionService
using Tiendara.CapaVisual.Utils;

namespace Tiendara.CapaVisual.Autenticacion
{
    public partial class RegistroTiendaPage : ContentPage
    {
        private readonly INegocioRepo _repo;
        private readonly SessionService _session;

        public RegistroTiendaPage(INegocioRepo repo, SessionService session)
        {
            InitializeComponent();
            _repo = repo;
            _session = session;
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
                PropietarioUsuarioId = _session.UsuarioId, // o _session.UsuarioActual!.Id si usas ese modelo
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
