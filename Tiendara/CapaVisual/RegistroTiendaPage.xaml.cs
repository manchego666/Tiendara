using System;
using Timer = System.Timers.Timer;



namespace Tiendara.CapaVisual;

public partial class RegistroTiendaPage : ContentPage
{

    private int pasoTutorial = 0;
    private readonly string[] mensajes = new[]
    {
        "¡Te ayudaré a organizar tu tienda! En el nombre puedes poner el que desees para tu tienda!!",
        "En tipo puedes elegir desde restaurante, hotdogs, sushi... ¡hasta una tienda enorme de ropa nivel internacional!",
        "Puedes especificar tu horario o dejarlo vacío. Luego podrás completarlo y llegar a más usuarios desde la iWeb."
    };

    private Timer parpadeo;

    public RegistroTiendaPage()
    {
        InitializeComponent();
        IniciarParpadeo();
    }

    private void IniciarParpadeo()
    {
        parpadeo = new Timer(600); // cada 0.6 segundos
        parpadeo.Elapsed += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                lblMensaje.Opacity = lblMensaje.Opacity == 1 ? 0.3 : 1;
            });
        };
        parpadeo.Start();
    }

    private void BtnSiguienteMensaje_Clicked(object sender, EventArgs e)
    {
        pasoTutorial++;

        if (pasoTutorial < mensajes.Length)
        {
            lblMensaje.Text = mensajes[pasoTutorial];
        }
        else
        {
            parpadeo?.Stop();
            lblMensaje.IsVisible = false;
            btnSiguienteMensaje.IsVisible = false;
            formularioRegistro.IsVisible = true;
        }
    }

    private async void OnRegistrarTiendaClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Éxito", "¡Tu tienda ha sido registrada!", "Continuar");
        await Navigation.PushAsync(new Tiendara.CapaVisual.tiendaradueno_home());
    }

}
