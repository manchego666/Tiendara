using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            // Desenfocar Home (workaround): marcamos otro y no lo usamos
            nav.Activo = "home"; // o "bell"; evita que Home quede activo

            nav.HomeClicked += async (_, __) => await Shell.Current.GoToAsync("//home");
            nav.WorldClicked += async (_, __) => await Navigation.PushAsync(new MapPage());
            nav.ChatClicked += async (_, __) => await Navigation.PushAsync(new ChatListPage());
            nav.BellClicked += async (_, __) => await DisplayAlert("ZDEV", "Notificaciones (en desarrollo)", "OK");

            // Menú lateral
            btnMenu.Clicked += async (_, __) => await menuLateral.ToggleMenu();

            // Ajuste dinámico de alto del panel
            SizeChanged += (_, __) => RecalcPanelHeight();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Carga región guardada
            var regStr = Preferences.Get("region", "MX·Sinaloa·Culiacán");
            var parts = regStr.Split('·');
            feed.SelectedRegion = new Ubicacion
            {
                Country = parts[0].Trim(),
                State = parts.Length > 1 ? parts[1].Trim() : "Sinaloa",
                City = parts.Length > 2 ? parts[2].Trim() : "Culiacán"
            };
            lblRegion.Text = feed.SelectedRegion.ToString();

            // Semilla de publicaciones fake
            feed.SeedIfEmpty();
            RecalcPanelHeight();
        }

        private void RecalcPanelHeight()
        {
            // margen top del contenedor + apron para no chocar con la bottom bar
            double top = 108, bottom = 88;
            double h = Height <= 0 ? 760 : Math.Max(420, Height - top - bottom);
            feed.PanelHeight = h;
        }

        private async void OnRegionTapped(object? sender, TappedEventArgs e)
        {
            var choice = await DisplayActionSheet("Selecciona región", "Cancelar", null,
                "MX · Sinaloa · Culiacán",
                "MX · CDMX · Coyoacán",
                "MX · Jalisco · Guadalajara");

            if (string.IsNullOrEmpty(choice) || choice == "Cancelar") return;

            var parts = choice.Split('·');
            feed.SelectedRegion = new Ubicacion
            {
                Country = parts[0].Trim(),
                State = parts[1].Trim(),
                City = parts[2].Trim()
            };
            lblRegion.Text = feed.SelectedRegion.ToString();
            Preferences.Set("region", lblRegion.Text);
            feed.SeedIfEmpty(); // repinta
        }

        private async void OnGpsClicked(object? sender, EventArgs e)
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) return;

                var loc = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
                if (loc == null) return;

                // TODO: reverse geocoding real. Por ahora lo dejamos fijo para no romper flujo.
                feed.SelectedRegion = new Ubicacion { Country = "MX", State = "Sinaloa", City = "Culiacán" };
                lblRegion.Text = feed.SelectedRegion.ToString();
                Preferences.Set("region", lblRegion.Text);
                feed.SeedIfEmpty();
            }
            catch { /* ignora por ahora */ }
        }
    }
}
