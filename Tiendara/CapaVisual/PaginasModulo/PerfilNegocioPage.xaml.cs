using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Tiendara.CapaLogica.Servicios;
using Tiendara.CapaContratos;
using Tiendara.CapaVisual.Utils;
using Tiendara.CapaVisual.Componentes.Publicaciones;
using Tiendara.CapaVisual.Autenticacion;
using Tiendara.CapaLogica.Servicios.Tiendara.CapaLogica.Servicios;

namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class PerfilNegocioPage : ContentPage
    {
        private readonly INegocioRepo _negocios;
        private readonly SessionService _session;

        private Guid _negocioId;

        public PerfilNegocioPage(INegocioRepo negocios, SessionService session)
        {
            _negocios = negocios;
            _session = session;

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
            await InicializarAsync();  // asegura que hay sesión y asigna _negocioId
        }


        private async Task InicializarAsync()
        {
            var usuario = _session.UsuarioActual;

            if (usuario == null)
            {
                await DisplayAlert("Sesión", "No hay sesión activa.", "OK");
                await Navigation.PopAsync();
                return;
            }

            // Traer negocio del usuario
            var lista = await _negocios.ListByUsuarioAsync(usuario.Id);
            if (lista.Count == 0)
            {
                await Navigation.PushAsync(new RegistroTiendaPage());
                return;
            }

            _negocioId = lista[0].Id; // <-- aquí ya sí asignas

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
