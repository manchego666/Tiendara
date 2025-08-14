using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Repos;
using Tiendara.CapaLogica.Servicios;
using Tiendara.CapaVisual.PaginasModulo;
using Tiendara.CapaVisual.Autenticacion;

namespace Tiendara.CapaVisual.Componentes;

public partial class MenuLateralTiendara : ContentView
{
    bool _abierto;
    bool _tiendaAbierta;

    public MenuLateralTiendara()
    {
        InitializeComponent();

        // Estado inicial: no intercepta toques
        this.InputTransparent = true;
        this.ZIndex = 0;

        menuPanel.IsVisible = false;
        menuPanel.InputTransparent = true;

        scrim.IsVisible = false;
        scrim.InputTransparent = true;

        // Botones
        btnPerfil.Clicked += OnPerfilClicked;
        btnPerfilTienda.Clicked += OnPerfilTiendaClicked;
        btnInventario.Clicked += OnInventarioClicked;
        btnRetiros.Clicked += OnRetirosClicked;
        btnEstadoTienda.Clicked += OnToggleTiendaClicked;
        btnConfig.Clicked += OnConfigClicked;
        btnAcerca.Clicked += OnAcercaClicked;
        btnCerrarSesion.Clicked += OnCerrarSesionClicked;
    }

    // ===== API pública =====
    public async Task ToggleMenu()
    {
        if (_abierto) await CerrarAsync();
        else await AbrirAsync();
    }

    public async Task AbrirAsync()
    {
        if (_abierto) return;
        _abierto = true;

        this.InputTransparent = false;
        this.ZIndex = 1000;

        scrim.IsVisible = true;
        scrim.InputTransparent = false;

        menuPanel.IsVisible = true;
        menuPanel.InputTransparent = false;

        var offset = menuPanel.Width > 0 ? -menuPanel.Width - 40 : -380;
        menuPanel.TranslationX = offset;

        var fade = scrim.FadeTo(1, 160, Easing.CubicOut);
        var slide = menuPanel.TranslateTo(0, 0, 260, Easing.CubicOut);
        await Task.WhenAll(fade, slide);
    }

    public async Task CerrarAsync()
    {
        if (!_abierto) return;
        _abierto = false;

        var offset = -(menuPanel.Width > 0 ? menuPanel.Width + 40 : 380);
        var slide = menuPanel.TranslateTo(offset, 0, 220, Easing.CubicIn);
        var fade = scrim.FadeTo(0, 180, Easing.CubicIn);
        await Task.WhenAll(slide, fade);

        menuPanel.IsVisible = false;
        menuPanel.InputTransparent = true;

        scrim.IsVisible = false;
        scrim.InputTransparent = true;

        this.InputTransparent = true;
        this.ZIndex = 0;
    }

    private async void OnScrimTapped(object? sender, EventArgs e) => await CerrarAsync();

    // ===== Navegación / acciones =====
    private async void OnPerfilClicked(object? s, EventArgs e)
    {
        await Navigation.PushAsync(new PerfilPage());
        await CerrarAsync();
    }

    private async void OnPerfilTiendaClicked(object? s, EventArgs e)
    {
        try
        {
            if (!SesionActual.Autenticado)
            {
                await Application.Current.MainPage.DisplayAlert("Perfil de negocio", "Inicia sesión primero.", "OK");
                return;
            }

            var repo = new NegocioRepo();
            var tiendas = await repo.ListByUsuarioAsync(SesionActual.UsuarioId);

            if (tiendas.Count == 0)
            {
                var crear = await Application.Current.MainPage.DisplayAlert(
                    "Perfil de negocio",
                    "Primero registra tu negocio para entrar al perfil.",
                    "Registrar", "Cancelar");

                if (crear)
                    await Navigation.PushAsync(new RegistroTiendaPage());
                return;
            }

            await Navigation.PushAsync(new PerfilNegocioPage(/*tiendas.First().Id*/));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            await CerrarAsync();
        }
    }

    private async void OnInventarioClicked(object? s, EventArgs e)
    {
        await Navigation.PushAsync(new InventarioPage());
        await CerrarAsync();
    }

    private async void OnRetirosClicked(object? s, EventArgs e)
    {
        await Navigation.PushAsync(new RetirosPage());
        await CerrarAsync();
    }

    private async void OnToggleTiendaClicked(object? s, EventArgs e)
    {
        _tiendaAbierta = !_tiendaAbierta;
        btnEstadoTienda.Text = _tiendaAbierta ? "Cerrar tienda" : "Abrir tienda";
        await Application.Current.MainPage.DisplayAlert(
            "Estado de la tienda",
            _tiendaAbierta ? "¡Tienda abierta!" : "Tienda cerrada.",
            "OK");
        await CerrarAsync();
    }

    private async void OnConfigClicked(object? s, EventArgs e)
    {
        // await Navigation.PushAsync(new ConfiguracionPage());
        await Application.Current.MainPage.DisplayAlert("Configuración", "Próximamente.", "OK");
        await CerrarAsync();
    }

    private async void OnAcercaClicked(object? s, EventArgs e)
    {
        // await Navigation.PushAsync(new AcercaPage());
        await Application.Current.MainPage.DisplayAlert("Acerca de", "Tiendara — beta.", "OK");
        await CerrarAsync();
    }

    private async void OnCerrarSesionClicked(object? s, EventArgs e)
    {
        var ok = await Application.Current.MainPage.DisplayAlert(
            "Cerrar sesión", "¿Seguro que deseas salir?", "Sí", "Cancelar");
        if (!ok) return;

        try { SesionActual.Cerrar(); } catch { /* no-op */ }

        await CerrarAsync();

        // Si tienes LoginPage/RegistroInicioPage, navega ahí
        // try { await Navigation.PushAsync(new LoginPage()); } catch {}
    }
}
