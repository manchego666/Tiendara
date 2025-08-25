using Microsoft.Maui.Controls;
using Tiendara.CapaContratos;
using Tiendara.CapaVisual.Componentes.Login;
using Tiendara.CapaVisual.Utils;
using Tiendara.CapaLogica.Servicios;

namespace Tiendara.CapaVisual.Autenticacion;

public partial class RegistroInicioPage : ContentPage
{
    public RegistroInicioPage()
    {
        InitializeComponent();

        // ⚡ Inyectamos los servicios al MonedaLoginView
        var authService = App.Services.GetRequiredService<IAuthService>();
        var sessionService = App.Services.GetRequiredService<SessionService>();
        monedaView.InitServices(authService, sessionService);
    }
}
