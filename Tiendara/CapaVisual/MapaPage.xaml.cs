using Microsoft.Maui.Controls;

namespace Tiendara.CapaVisual
{
    public partial class MapaPage : ContentPage
    {
        public MapaPage()
        {
            InitializeComponent();
            CargarMapa();
        }

        private void CargarMapa()
        {
            var htmlMapa = @"
<!DOCTYPE html>
<html>
<head>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<link rel='stylesheet' href='https://unpkg.com/leaflet/dist/leaflet.css'/>
<style>
  #map { height: 100vh; width: 100%; }
</style>
</head>
<body>
<div id='map'></div>
<script src='https://unpkg.com/leaflet/dist/leaflet.js'></script>
<script>
  var map = L.map('map').setView([24.799, -107.389], 13);
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors'
  }).addTo(map);
</script>
</body>
</html>";

            webMapa.Source = new HtmlWebViewSource
            {
                Html = htmlMapa
            };
        }
    }
}
