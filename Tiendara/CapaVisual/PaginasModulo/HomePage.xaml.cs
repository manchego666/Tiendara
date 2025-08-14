using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            nav.HomeClicked += (_, __) => { /* ya en Home */ };
           // nav.MensajesClicked += async (_, __) => await DisplayAlert("ZDEV - 2025", "Mensajes (en desarrollo).", "OK");
           // nav.NotificacionesClicked += async (_, __) => await DisplayAlert("ZDEV - 2025", "Notificaciones (en desarrollo).", "OK");

            btnMenu.Clicked += async (_, __) => await menuLateral.ToggleMenu();
        }
    }
}
