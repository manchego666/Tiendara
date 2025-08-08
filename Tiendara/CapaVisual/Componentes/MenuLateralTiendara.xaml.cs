using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace Tiendara.CapaVisual.Componentes;

public partial class MenuLateralTiendara : ContentView
{
    private bool tiendaAbierta = false;
    private bool estaAbierto = false;
    private bool mapaCargado = false;   //para no recargar cada vez

    public MenuLateralTiendara()
    {
        InitializeComponent();

        var tapHamburguesa = new TapGestureRecognizer();
        tapHamburguesa.Tapped += async (s, e) => await ToggleMenu();
        btnHamburguesa.GestureRecognizers.Add(tapHamburguesa);

        var tapCerrar = new TapGestureRecognizer();
        tapCerrar.Tapped += async (s, e) => await CerrarMenu();
        btnCerrarMenu.GestureRecognizers.Add(tapCerrar);

        btnEstadoTienda.Clicked += OnToggleTiendaClicked;

        // Botón mapa
        btnMapa.Clicked += OnMapaClicked;
    }

    // ===== Menú lateral =====
    public async Task ToggleMenu()
    {
        if (!estaAbierto) await AbrirMenu();
        else await CerrarMenu();
    }

    public async Task AbrirMenu()
    {
        if (estaAbierto) return;
        menuLateral.IsVisible = true;
        await menuLateral.TranslateTo(0, 0, 300, Easing.SinOut);
        estaAbierto = true;
    }

    public async Task CerrarMenu()
    {
        if (!estaAbierto) return;
        await menuLateral.TranslateTo(-250, 0, 300, Easing.SinIn);
        menuLateral.IsVisible = false;
        estaAbierto = false;
    }

    private void ActualizarTextoBotonTienda()
        => btnEstadoTienda.Text = tiendaAbierta ? "Cerrar tienda" : "Abrir tienda";

    private async void OnToggleTiendaClicked(object sender, EventArgs e)
    {
        tiendaAbierta = !tiendaAbierta;
        ActualizarTextoBotonTienda();
        await Application.Current.MainPage.DisplayAlert(
            "Estado de la tienda",
            tiendaAbierta ? "¡Tienda abierta!" : "Tienda cerrada.",
            "OK");
    }

    // ===== Mapa (WebView + Leaflet) =====
    private async void OnMapaClicked(object? sender, EventArgs e)
    {
        panelMapa.IsVisible = true;

        if (!mapaCargado)
        {
            var html = @"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'/>
<meta name='viewport' content='width=device-width,initial-scale=1,maximum-scale=1'/>
<link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/>
<style>html,body,#map{height:100%;margin:0}</style>
</head>
<body>
<div id='map'></div>
<script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
<script>
  var lat=24.799, lng=-107.389, z=13; // Culiacán ejemplo
  var map=L.map('map').setView([lat,lng],z);
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',{
    maxZoom:19, attribution:'&copy; OpenStreetMap'
  }).addTo(map);
  L.marker([lat,lng]).addTo(map).bindPopup('Aquí la tienda');
</script>
</body>
</html>";
            webMapa.Source = new HtmlWebViewSource { Html = html };
            mapaCargado = true;
        }

        if (estaAbierto) await CerrarMenu();
    }

    private void OnCerrarPanelMapaTapped(object sender, TappedEventArgs e)
    {
        panelMapa.IsVisible = false;
    }
}
