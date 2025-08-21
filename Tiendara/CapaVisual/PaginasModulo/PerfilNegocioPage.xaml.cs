using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Tiendara.CapaLogica.Servicios;
using Tiendara.CapaContratos;
using Tiendara.CapaVisual.Utils;
using Tiendara.CapaVisual.Componentes.Publicaciones;

namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class PerfilNegocioPage : ContentPage
    {
        private readonly INegocioRepo _negocios;
        private readonly Utils.SessionService _session;
        private readonly Guid _negocioId;

        public PerfilNegocioPage(Guid negocioId/* Guid negocioId si quieres */)
        {
            InitializeComponent();

            // Botón menú
            btnMenu.Clicked += async (_, __) => await menuLateral.ToggleMenu();

            // Barra inferior
            nav.HomeClicked += async (_, __) =>
                await DisplayAlert("Tiendara", "Estás en Perfil de negocio.", "OK");

            // Feed (eventos)
            pub.CommentRequested += OnPubCommentRequested;
            pub.ContactRequested += OnPubContactRequested;
            pub.ProfileRequested += OnPubProfileRequested;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            var negocio = await _negocios.GetByIdAsync(_negocioId);
            if (negocio == null)
            {
                await DisplayAlert("Error", "No se encontró el negocio.", "OK");
                await Navigation.PopAsync();
                return;
            }

            // Asigna los datos del negocio a la UI
            portada.Titulo = string.IsNullOrWhiteSpace(negocio.Nombre) ? "Mi Negocio" : negocio.Nombre.Trim();
            portada.Subtitulo = "Negocio";

            // Feed modo Tienda
            pub.Modo = PublicacionesModo.Tienda;
            pub.TiendaId = negocio.Id;
            pub.TiendaNombre = negocio.Nombre ?? "Mi Negocio";

            // Autor (quien publica) = usuario en sesión
            var usuario = _session.UsuarioActual;
            pub.AutorId = usuario?.Id ?? Guid.Empty;
            pub.AutorNombre = usuario?.Nombre ?? "Usuario";
        }

        // ===== Acciones del feed =====
        private async void OnPubCommentRequested(object? sender, PublicacionItem item)
            => await DisplayAlert("Comentar", $"Comentar publicación de {item.AutorLinea}", "OK");

        private async void OnPubContactRequested(object? sender, PublicacionItem item)
            => await DisplayAlert("Contactar", $"Abrir chat con {item.AutorLinea}", "OK");

        private async void OnPubProfileRequested(object? sender, PublicacionItem item)
            => await DisplayAlert("Perfil", $"Ir al perfil de {item.AutorLinea}", "OK");
    }
}
