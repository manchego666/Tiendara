using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading.Tasks;
using Tiendara.CapaSql.Startup;
using Tiendara.CapaLogica.Servicios;
using Tiendara.CapaVisual.Componentes.Portada;
using Tiendara.CapaVisual.Componentes.Publicaciones;
using Tiendara.CapaContratos;
using Tiendara.CapaSql.Conexion;



namespace Tiendara.CapaVisual.PaginasModulo;

public partial class PerfilPage : ContentPage
{
    private readonly INegocioRepo _negocios;
    private readonly IUsuarioRepo _usuarios;
    private readonly SessionService _sessionService;
    private readonly IFotoApi _fotos;

    public PerfilPage(INegocioRepo negocios, IUsuarioRepo usuarios, SessionService sessionService, IFotoApi fotos)
    {
        InitializeComponent();
        _negocios = negocios;
        _usuarios = usuarios;
        _sessionService = sessionService;
        _fotos = fotos;

        nav.Activo = "none";
        nav.HomeClicked += async (_, __) => await Navigation.PopToRootAsync();
        nav.WorldClicked += async (_, __) => await Navigation.PushAsync(new MapPage());

        btnMenu.Clicked += async (_, __) => await menuLateral.ToggleMenu();

        portada.Modo = PortadaModo.Usuario;
        portada.VerFotoSolicitado += async (_, __) => await VerFotoAsync();
        portada.CambiarFotoSolicitado += async (_, __) => await CambiarFotoAsync();

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
        var u = _sessionService.UsuarioActual;
        if (u == null)
        {
            await DisplayAlert("Sesión", "No hay sesión activa. Inicia sesión de nuevo.", "OK");
            return;
        }

        // Portada
        portada.Titulo = (u.Nombre ?? string.Empty).Trim();
        portada.Subtitulo = string.IsNullOrWhiteSpace(u.Email) ? "Usuario Tiendara+" : u.Email;

        // Solo el nombre de archivo, PortadaPerfilView se encarga de la ruta
        portada.FotoPath = u.AvatarPath;   // puede venir null o relativo

        try
        {
            var tiendas = await _negocios.ListByUsuarioAsync(u.Id);
            portada.TiendasCount = tiendas?.Count ?? 0;
        }
        catch
        {
            portada.TiendasCount = 0;
        }
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
        var abs = Tiendara.CapaLogica.Infra.BackendConfig.ToAbsoluteMediaUrl(portada.FotoPath);
        if (string.IsNullOrWhiteSpace(abs))
        { await DisplayAlert("Foto", "Sin foto.", "OK"); return; }

        var img = new Image { Aspect = Aspect.AspectFit, Source = ImageSource.FromUri(new Uri(abs)), BackgroundColor = Colors.Black };
        var viewer = new ContentPage { Title = "Foto de perfil", BackgroundColor = Colors.Black, Content = new Grid { Children = { img } } };
        viewer.ToolbarItems.Add(new ToolbarItem("Cerrar", null, async () => await Navigation.PopModalAsync()));
        img.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(async () => await Navigation.PopModalAsync()) });
        await Navigation.PushModalAsync(new NavigationPage(viewer) { BarTextColor = Colors.White, BarBackgroundColor = Colors.Black });
    }


    private async Task CambiarFotoAsync()
    {
        if (!await Tiendara.CapaVisual.Utils.Permisos.EnsureFotoAsync())
        { await DisplayAlert("Permisos", "Necesito acceso a fotos.", "OK"); return; }

        var pick = await FilePicker.PickAsync(new PickOptions { PickerTitle = "Elige imagen", FileTypes = FilePickerFileType.Images });
        if (pick is null) return;

        var u = _sessionService.UsuarioActual;
        if (u is null) { await DisplayAlert("Sesión", "Sin sesión activa.", "OK"); return; }

        await using var s = await pick.OpenReadAsync();
        var url = await _fotos.SubirAvatarAsync(u.Id, s, pick.FileName);

        // Refresca al instante (la vista arma la URL y cache-buster)
        portada.FotoPath = url;

        await DisplayAlert("Perfil", "Foto actualizada.", "OK");
    }

}
