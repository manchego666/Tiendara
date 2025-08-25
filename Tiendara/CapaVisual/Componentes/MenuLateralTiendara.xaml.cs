using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaLogica.Servicios; // SessionService
using Tiendara.CapaVisual.Autenticacion;
using Tiendara.CapaVisual.PaginasModulo;

namespace Tiendara.CapaVisual.Componentes
{
    public partial class MenuLateralTiendara : ContentView
    {
        bool _abierto;
        bool _tiendaAbierta;
        private static Task Alert(string title, string message, string cancel = "OK")
    => Application.Current.MainPage.DisplayAlert(title, message, cancel);

        private static Task<bool> Confirm(string title, string message, string accept = "OK", string cancel = "Cancelar")
            => Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);

        public MenuLateralTiendara()
        {
            InitializeComponent();

            // Estado inicial
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

        // ===== Helper para DI =====
        private T Get<T>() where T : notnull
        {
            var sp = Application.Current?.Handler?.MauiContext?.Services;
            if (sp == null) throw new InvalidOperationException("Servicios no disponibles aún (MauiContext).");
            return sp.GetRequiredService<T>();
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
            var perfilPage = Get<PerfilPage>();
            await Navigation.PushAsync(perfilPage);
            await CerrarAsync();
        }

        private async void OnPerfilTiendaClicked(object? s, EventArgs e)
        {
            try
            {
                var sesion = Get<SessionService>();
                var usuario = sesion.UsuarioActual;
                if (usuario is null)
                {
                    await Alert("Perfil de negocio", "Inicia sesión primero.");
                    return;
                }

                var negociosSrv = Get<INegocioRepo>();
                var tiendas = await negociosSrv.ListByUsuarioAsync(usuario.Id);

                if (tiendas is null || tiendas.Count == 0)
                {
                    var crear = await Confirm(
                        "Perfil de negocio",
                        "Primero registra tu negocio para entrar al perfil.",
                        "Registrar", "Cancelar");

                    if (crear)
                    {
                        var reg = Get<RegistroTiendaPage>();   // DI crea la página con deps
                        await Navigation.PushAsync(reg);
                    }
                    return;
                }

                var page = Get<PerfilNegocioPage>();          // DI inyecta INegocioRepo, SessionService, IFotoApi
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await Alert("Error", ex.Message);
            }
            finally
            {
                await CerrarAsync();
            }
        }










        private async void OnInventarioClicked(object? s, EventArgs e)
        {
            var inventarioPage = Get<InventarioPage>();
            await Navigation.PushAsync(inventarioPage);
            await CerrarAsync();
        }

        private async void OnRetirosClicked(object? s, EventArgs e)
        {
            var retirosPage = Get<RetirosPage>();
            await Navigation.PushAsync(retirosPage);
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

            try
            {
                var sesion = Get<SessionService>();
                sesion.Clear();
            }
            catch { /* no-op */ }

            await CerrarAsync();

            Application.Current.MainPage = new Tiendara.CapaVisual.Autenticacion.RegistroInicioPage();
            // Redirección tras logout (ajusta según tus rutas/pages reales)
            // try { await Shell.Current.GoToAsync("//login"); } catch { }
            // o: await Navigation.PushAsync(new RegistroInicioPage());
        }
    }
}
