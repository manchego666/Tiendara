using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Tiendara.CapaLogica.Servicios;
using Tiendara.CapaContratos;
using Tiendara.CapaVisual.Utils;
using Tiendara.CapaVisual.Componentes.Publicaciones;
using Tiendara.CapaVisual.Autenticacion;
using Microsoft.Maui.Storage;
using Tiendara.CapaVisual.Componentes.Portada;
using Microsoft.Extensions.DependencyInjection;



namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class PerfilNegocioPage : ContentPage
    {
        private readonly INegocioRepo _negocios;
        private readonly SessionService _session;
        private readonly IFotoApi _fotos;
        private Guid _negocioId;




        public PerfilNegocioPage(INegocioRepo negocios, SessionService session, IFotoApi fotos)
        {
            InitializeComponent();
            _negocios = negocios;
            _session = session;
            _fotos = fotos;

            portada.Modo = PortadaModo.Tienda;
            btnMenu.Clicked += async (_, __) => await menuLateral.ToggleMenu();
            nav.HomeClicked += async (_, __) => await DisplayAlert("Tiendara", "Estás en Perfil de negocio.", "OK");
            portada.VerFotoSolicitado += async (_, __) => await VerLogoAsync();
            portada.EditarDatosClicked += async (_, __) => await EditarDatosNegocioAsync();
            portada.EditarTemasClicked += async (_, __) => await EditarTemasNegocioAsync();



            pub.CommentRequested += OnPubCommentRequested;
            pub.ContactRequested += OnPubContactRequested;
            pub.ProfileRequested += OnPubProfileRequested;

            portada.CambiarFotoSolicitado += async (_, __) => await CambiarLogoAsync();
        }

        private async Task EditarDatosNegocioAsync()
        {
            await DisplayAlert("Negocio", "Abrir editor de datos (en progreso).", "OK");
            // TODO: Navigation.PushAsync(new EditarNegocioPage(_negocioId));
        }

        private async Task EditarTemasNegocioAsync()
        {
            await DisplayAlert("Temas", "Abrir editor de temas (en progreso).", "OK");
            // TODO: Navigation.PushAsync(new EditarTemasPage(_negocioId));
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
            if (lista == null || lista.Count == 0)
            {
                // ⬇️ Opción 1: resolver desde el MauiContext (ServiceProvider)
                var sp = Application.Current?.Handler?.MauiContext?.Services
                         ?? throw new InvalidOperationException("Servicios no disponibles (MauiContext).");

                var reg = sp.GetRequiredService<RegistroTiendaPage>();
                await Navigation.PushAsync(reg);
                return;
            }

            _negocioId = lista[0].Id;
            await CargarDatosAsync();
        }
        private async Task VerLogoAsync()
        {
            var abs = Tiendara.CapaLogica.Infra.BackendConfig.ToAbsoluteMediaUrl(portada.FotoPath);
            if (string.IsNullOrWhiteSpace(abs))
            { await DisplayAlert("Logo", "Sin logo.", "OK"); return; }

            var img = new Image
            {
                Aspect = Aspect.AspectFit,
                Source = ImageSource.FromUri(new Uri(abs)),
                BackgroundColor = Colors.Black
            };

            var viewer = new ContentPage
            {
                Title = "Logo",
                BackgroundColor = Colors.Black,
                Content = new Grid { Children = { img } }
            };
            viewer.ToolbarItems.Add(new ToolbarItem("Cerrar", null, async () => await Navigation.PopModalAsync()));
            img.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(async () => await Navigation.PopModalAsync()) });

            await Navigation.PushModalAsync(new NavigationPage(viewer)
            {
                BarTextColor = Colors.White,
                BarBackgroundColor = Colors.Black
            });
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

            // ✅ ya puedes usarlo
            portada.FotoPath = negocio.LogoPath;   // ruta relativa o null

            portada.Titulo = string.IsNullOrWhiteSpace(negocio.Nombre) ? "Mi Negocio" : negocio.Nombre.Trim();
            portada.Subtitulo = "Negocio";

            pub.Modo = PublicacionesModo.Tienda;
            pub.TiendaId = negocio.Id;
            pub.TiendaNombre = negocio.Nombre ?? "Mi Negocio";

            var usuario = _session.UsuarioActual;
            pub.AutorId = usuario?.Id ?? Guid.Empty;
            pub.AutorNombre = usuario?.Nombre ?? "Usuario";
        }


        private async Task CambiarLogoAsync()
        {
            var pick = await FilePicker.PickAsync(new PickOptions { PickerTitle = "Elige el logo", FileTypes = FilePickerFileType.Images });
            if (pick is null) return;

            await using var s = await pick.OpenReadAsync();
            var url = await _fotos.SubirLogoAsync(_negocioId, s, pick.FileName);

            portada.FotoPath = url;
            await DisplayAlert("Negocio", "Logo actualizado.", "OK");
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
