using Microsoft.Maui.Controls;

namespace Tiendara;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnRegistroClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CapaVisual.Autenticacion.RegistroInicioPage());
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CapaVisual.PaginasModulo.HomePage());
    }
}
