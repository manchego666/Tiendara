using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Repos;
using Tiendara.CapaLogica.Servicios;
using Tiendara.CapaVisual.Componentes.Portada;
using Tiendara.CapaVisual.Componentes.Publicaciones;

namespace Tiendara.CapaVisual.PaginasModulo;

public partial class PerfilPage : ContentPage
{
    private readonly INegocioRepo _negocios = new NegocioRepo();
    private readonly IUsuarioRepo _usuarios = new UsuarioRepo();

    public PerfilPage()
    {
        InitializeComponent();

        nav.Activo = "none"; // desactiva todos
        nav.HomeClicked += async (_, __) => await Navigation.PopToRootAsync();
        nav.WorldClicked += async (_, __) => await Navigation.PushAsync(new MapPage());


        // Menú lateral
        btnMenu.Clicked += async (_, __) => await menuLateral.ToggleMenu();

        // Portada (eventos)
        portada.VerFotoSolicitado += async (_, __) => await VerFotoAsync();
        portada.CambiarFotoSolicitado += async (_, __) => await CambiarFotoAsync();
        portada.EditarDatosClicked += async (_, __) =>
            await DisplayAlert("ZDEV - 2025", "Editar datos (en desarrollo).", "OK");
        portada.EditarTemasClicked += async (_, __) =>
            await DisplayAlert("ZDEV - 2025", "Editar temas (en desarrollo).", "OK");

        // Feed (eventos)
        pub.CommentRequested += OnPubCommentRequested;
        pub.ContactRequested += OnPubContactRequested;
        pub.ProfileRequested += OnPubProfileRequested;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SincronizarConSesionAsync();
    }

    private async Task SincronizarConSesionAsync()
    {
        var u = SesionActual.Usuario;
        if (u == null)
        {
            await DisplayAlert("Sesión", "No hay sesión activa. Inicia sesión de nuevo.", "OK");
            return;
        }

        // Portada
        portada.Titulo = (u.Nombre ?? string.Empty).Trim();
        portada.Subtitulo = string.IsNullOrWhiteSpace(u.Email) ? "Usuario Tiendara+" : u.Email;
        portada.FotoPath = (!string.IsNullOrWhiteSpace(u.Foto) && File.Exists(u.Foto)) ? u.Foto : null;

        try
        {
            var tiendas = await _negocios.ListByUsuarioAsync(u.Id);
            portada.TiendasCount = tiendas?.Count ?? 0;
        }
        catch { portada.TiendasCount = 0; }
        portada.NoticiasCount = 0;

        // Feed (identidad)
        pub.Modo = PublicacionesModo.Usuario;
        pub.AutorId = u.Id;
        pub.AutorNombre = u.Nombre ?? "Usuario";
        pub.TiendaId = null;
        pub.TiendaNombre = null;
    }

    // ===== Acciones del feed =====
    private async void OnPubCommentRequested(object? sender, PublicacionItem item)
        => await DisplayAlert("Comentar", $"Comentar publicación de {item.AutorLinea}", "OK");

    private async void OnPubContactRequested(object? sender, PublicacionItem item)
        => await DisplayAlert("Contactar", $"Abrir chat con {item.AutorLinea}", "OK");

    private async void OnPubProfileRequested(object? sender, PublicacionItem item)
        => await DisplayAlert("Perfil", $"Ir al perfil de {item.AutorLinea}", "OK");

    // ===== Foto de perfil =====
    private async Task VerFotoAsync()
    {
        var src = (!string.IsNullOrWhiteSpace(portada.FotoPath) && File.Exists(portada.FotoPath))
            ? ImageSource.FromFile(portada.FotoPath)
            : ImageSource.FromFile("avatar_default.png");

        var img = new Image { Aspect = Aspect.AspectFit, Source = src, BackgroundColor = Colors.Black };
        var viewer = new ContentPage
        {
            Title = "Foto de perfil",
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

    private async Task CambiarFotoAsync()
    {
        if (!await Tiendara.CapaVisual.Utils.Permisos.EnsureFotoAsync())
        {
            await DisplayAlert("Permisos", "Necesito acceso a cámara y fotos.", "OK");
            return;
        }

        var action = await DisplayActionSheet("Foto de perfil", "Cancelar", null, "Desde cámara", "Desde galería");
        try
        {
            FileResult? fr = action switch
            {
                "Desde cámara" => await MediaPicker.CapturePhotoAsync(),
                "Desde galería" => await MediaPicker.PickPhotoAsync(),
                _ => null
            };
            if (fr == null) return;

            var u = SesionActual.Usuario;
            if (u == null)
            {
                await DisplayAlert("Sesión", "Sin sesión activa.", "OK");
                return;
            }

            var dir = FileSystem.AppDataDirectory;
            var filename = $"avatar_{u.Id}.jpg";
            var dest = Path.Combine(dir, filename);

            using (var src = await fr.OpenReadAsync())
            using (var dst = File.Create(dest))
                await src.CopyToAsync(dst);

            portada.FotoPath = dest;

            u.Foto = dest;
            await _usuarios.UpdateAsync(u);

            await DisplayAlert("Perfil", "Foto actualizada.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
