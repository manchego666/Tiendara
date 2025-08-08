using Microsoft.Maui.Controls;

namespace Tiendara.CapaVisual.Componentes;

public partial class MenuLateralTiendara : ContentView
{
    private bool tiendaAbierta = false;
    private bool estaAbierto = false;

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

        ActualizarTextoBotonTienda();
    }

    public async Task ToggleMenu()
    {
        if (!estaAbierto)
            await AbrirMenu();
        else
            await CerrarMenu();
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
    {
        btnEstadoTienda.Text = tiendaAbierta ? "Cerrar tienda" : "Abrir tienda";
    }

    private async void OnToggleTiendaClicked(object sender, EventArgs e)
    {
        tiendaAbierta = !tiendaAbierta;
        ActualizarTextoBotonTienda();

        string estado = tiendaAbierta ? "¡Tienda abierta!" : "Tienda cerrada.";
        await Application.Current.MainPage.DisplayAlert("Estado de la tienda", estado, "OK");
    }
}
