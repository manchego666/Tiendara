using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class MapPage : ContentPage
    {
        bool _ready;

        public MapPage()
        {
            InitializeComponent();

            // HTML del mapa
            mapWeb.Source = new HtmlWebViewSource { Html = BuildHtml() };
            mapWeb.Navigated += (_, __) => _ready = true;

            // Barra inferior
            nav.Activo = "world";
            nav.HomeClicked += async (_, __) => await Navigation.PopToRootAsync();
            nav.WorldClicked += (_, __) => { /* ya aquí */ };
            nav.ChatClicked += async (_, __) => await Navigation.PushAsync(new ChatListPage());
            nav.BellClicked += async (_, __) => await DisplayAlert("ZDEV", "Notificaciones (en desarrollo).", "OK");

            // Menú lateral
            btnMenu.Clicked += async (_, __) => await menuLateral.ToggleMenu();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Espera a que cargue el WebView
            for (int i = 0; i < 60 && !_ready; i++) await Task.Delay(50);

            // Centro por defecto + POIs demo
            await EvalAsync("centerOn(24.806, -107.394, 13)");
            await EvalAsync("setPOIs([{lat:24.809,lng:-107.395,name:'Taco Pro',cat:'taco'},{lat:24.802,lng:-107.389,name:'Mini Abarrotes',cat:'store'},{lat:24.812,lng:-107.401,name:'Clínica Zeus',cat:'doctor'}])");

            // Centra en usuario si hay permiso
            await CenterOnUserAsync();
        }

        // ===== FABs =====

        async void OnMyLocationClicked(object? sender, EventArgs e) => await CenterOnUserAsync();

        async void OnAddStoreClicked(object? sender, EventArgs e)
        {
            // Elige categoría
            var cat = await DisplayActionSheet("Tipo de negocio", "Cancelar", null, "Tienda", "Tacos", "Doctor");
            if (string.IsNullOrEmpty(cat) || cat == "Cancelar") return;

            var key = cat switch
            {
                "Tacos" => "taco",
                "Doctor" => "doctor",
                _ => "store"
            };

            // Agrega en el centro actual
            await EvalAsync($@"
                (function(){{
                    const c = map.getCenter();
                    addPOI(c.lat, c.lng, '{Escape(cat)} nuevo', '{key}');
                }})();
            ");
        }

        // ===== Util =====

        async Task CenterOnUserAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) return;

                var loc = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
                if (loc != null)
                    await EvalAsync($"centerOn({loc.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{loc.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},15)");
            }
            catch { }
        }

        Task<string> EvalAsync(string js) => mapWeb.EvaluateJavaScriptAsync(js);

        static string Escape(string s) => s.Replace("\\", "\\\\").Replace("'", "\\'");

        string BuildHtml()
        {
            // Tres ICONOS SVG (tienda, tacos, doctor). Si luego quieres PNG tuyos, te cambio a base64 en 1 línea.
            const string STORE_SVG = @"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 64 64'>
  <defs><linearGradient id='g' x1='0' y1='0' x2='0' y2='1'><stop offset='0' stop-color='#FFB703'/><stop offset='1' stop-color='#FB8500'/></linearGradient></defs>
  <circle cx='32' cy='32' r='30' fill='white' stroke='#333' stroke-width='2'/>
  <rect x='12' y='28' width='40' height='24' rx='4' fill='url(#g)' stroke='#333' stroke-width='2'/>
  <rect x='16' y='18' width='32' height='12' rx='4' fill='#023047'/>
  <rect x='18' y='40' width='12' height='12' rx='2' fill='white' stroke='#333' stroke-width='2'/>
  <rect x='34' y='40' width='12' height='12' rx='2' fill='white' stroke='#333' stroke-width='2'/>
</svg>";

            const string TACO_SVG = @"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 64 64'>
  <circle cx='32' cy='32' r='30' fill='white' stroke='#333' stroke-width='2'/>
  <path d='M12 36a20 20 0 0 1 40 0' fill='#F4D35E' stroke='#333' stroke-width='2'/>
  <path d='M16 36c8-10 24-10 32 0' fill='none' stroke='#E63946' stroke-width='3'/>
  <circle cx='24' cy='34' r='2' fill='#2A9D8F'/><circle cx='40' cy='34' r='2' fill='#2A9D8F'/>
</svg>";

            const string DOCTOR_SVG = @"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 64 64'>
  <circle cx='32' cy='32' r='30' fill='white' stroke='#333' stroke-width='2'/>
  <rect x='20' y='20' width='24' height='24' rx='4' fill='#8ECAE6' stroke='#333' stroke-width='2'/>
  <rect x='30' y='16' width='4' height='32' fill='#333'/>
  <rect x='16' y='30' width='32' height='4' fill='#333'/>
</svg>";

            var html = new StringBuilder();
            html.AppendLine(@"<!doctype html><html><head>
<meta name='viewport' content='width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no'>
<link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'>
<style>
html,body,#map{height:100%;margin:0;background:#0D0F15}
.leaflet-control-attribution{display:none}
</style>
</head><body>
<div id='map'></div>
<script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
<script>
let map; let markers=[];

// ICONOS
const STORE_ICON  = L.icon({iconUrl:'data:image/svg+xml;utf8,' + encodeURIComponent(`" + STORE_SVG + @"`),  iconSize:[36,36], iconAnchor:[18,34], popupAnchor:[0,-28]});
const TACO_ICON   = L.icon({iconUrl:'data:image/svg+xml;utf8,' + encodeURIComponent(`" + TACO_SVG + @"`),  iconSize:[36,36], iconAnchor:[18,34], popupAnchor:[0,-28]});
const DOCTOR_ICON = L.icon({iconUrl:'data:image/svg+xml;utf8,' + encodeURIComponent(`" + DOCTOR_SVG + @"`),  iconSize:[36,36], iconAnchor:[18,34], popupAnchor:[0,-28]});

function iconFor(cat){
  switch(cat){
    case 'taco':   return TACO_ICON;
    case 'doctor': return DOCTOR_ICON;
    default:       return STORE_ICON;
  }
}

function init(){
  map = L.map('map', { zoomControl: false, touchZoom: true, scrollWheelZoom: true, doubleClickZoom: true }).setView([24.806,-107.394], 13);
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',{maxZoom:19}).addTo(map);

  // Agrego control + / - en bottom-left para que no choque con tus FABs
  L.control.zoom({ position: 'bottomleft' }).addTo(map);
}

function clearMarkers(){ for(const m of markers){ m.remove(); } markers=[]; }

function addPOI(lat,lng,name,cat){
  const m=L.marker([lat,lng],{icon:iconFor(cat)}).addTo(map);
  if(name){ m.bindPopup(name); }
  markers.push(m);
}

function setPOIs(arr){ clearMarkers(); for(const s of arr){ addPOI(s.lat,s.lng,s.name,s.cat); } }

function centerOn(lat,lng,zoom){ map.setView([lat,lng], zoom||15); }

document.addEventListener('DOMContentLoaded', init);
</script>
</body></html>");

            return html.ToString();
        }
    }
}
