using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Repos;
using Tiendara.CapaLogica.Servicios;
using Tiendara.CapaVisual.Componentes.Publicaciones;

namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class PerfilNegocioPage : ContentPage
    {
        private readonly INegocioRepo _negocios = new NegocioRepo();

        public PerfilNegocioPage(/* Guid negocioId si quieres */)
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
            var u = SesionActual.Usuario;
            if (u == null) return;

            // Si tienes tiendas registradas, toma la primera
            try
            {
                var tiendas = await _negocios.ListByUsuarioAsync(u.Id);
                if (tiendas != null && tiendas.Count > 0)
                {
                    var t = tiendas[0];
                    portada.Titulo = string.IsNullOrWhiteSpace(t.Nombre) ? "Mi Negocio" : t.Nombre.Trim();
                    portada.Subtitulo = "Negocio";

                    // Si luego agregas LogoPath:
                    // if (!string.IsNullOrWhiteSpace(t.LogoPath) && File.Exists(t.LogoPath))
                    //     portada.FotoPath = t.LogoPath;

                    // Feed modo Tienda
                    pub.Modo = PublicacionesModo.Tienda;
                    pub.TiendaId = t.Id;
                    pub.TiendaNombre = t.Nombre ?? "Mi Negocio";

                    // Autor (quien publica) = usuario en sesión
                    pub.AutorId = u.Id;
                    pub.AutorNombre = u.Nombre ?? "Usuario";
                }
                else
                {
                    // Sin tiendas: deja textos por defecto
                    portada.Titulo = "Mi Negocio";
                    portada.Subtitulo = "Negocio";
                }
            }
            catch
            {
                portada.Titulo = "Mi Negocio";
                portada.Subtitulo = "Negocio";
            }
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
