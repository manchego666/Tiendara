using System;
using Microsoft.Maui.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace Tiendara;

public partial class RegistroInicioPage : ContentPage
{
    private int pasoTutorial = 0;
    private CancellationTokenSource? animacionBoton;
    private CancellationTokenSource? animacionCuadro;

    private readonly string[] pasos = new string[]
    {
        "Primero deberás registrarte para comenzar a usar tu app Tiendara!",
        "No esperes en tener tu negocio en orden y en honda con Tiendara!"
    };

    public RegistroInicioPage()
    {
        InitializeComponent();

        // Ocultar al inicio
        panelFormulario.IsVisible = false;
        btnMostrarRegistro.IsVisible = false;

        // Parpadeo cuadro
        animacionCuadro = new CancellationTokenSource();

        // Avanza al tocar
        var tap = new TapGestureRecognizer();
        tap.Tapped += AvanzarTutorial;
        contenedorAnimado.GestureRecognizers.Add(tap);
    }

    // Sobrescribe el evento de cargado
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Espera a que cargue todo
        await Task.Delay(300);

        // Comienza animaciones
        _ = Parpadear(contenedorAnimado, animacionCuadro.Token);
    }

    private async void AnimarTutorial()
    {
        while (contenedorAnimado.IsVisible)
        {
            await Task.WhenAll(
                contenedorAnimado.FadeTo(0.2, 500),
                lblMensajeTutorial.FadeTo(0.2, 500)
            );

            await Task.WhenAll(
                contenedorAnimado.FadeTo(1, 500),
                lblMensajeTutorial.FadeTo(1, 500)
            );
        }
    }

    private async void AvanzarTutorial(object? sender, EventArgs e)
    {
        if (pasoTutorial < pasos.Length)
        {
            lblMensajeTutorial.Text = pasos[pasoTutorial];
            pasoTutorial++;
        }
        else
        {
            // Detener parpadeo del cuadro
            animacionCuadro?.Cancel();

            // Desvanecer encabezado
            await contenedorAnimado.FadeTo(0, 500);
            contenedorAnimado.IsVisible = false;

            // Mostrar botón y animarlo
            btnMostrarRegistro.IsVisible = true;
            animacionBoton = new CancellationTokenSource();
            _ = Parpadear(btnMostrarRegistro, animacionBoton.Token);
        }
    }

    private async Task Parpadear(VisualElement elemento, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await elemento.FadeTo(0.2, 500);
            await elemento.FadeTo(1, 500);
        }
    }

    private void MostrarFormulario(object sender, EventArgs e)
    {
        animacionBoton?.Cancel();
        btnMostrarRegistro.IsVisible = false;
        panelFormulario.IsVisible = true;
    }

    private async void OnRegistrarseClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Registro", "¡Gracias por registrarte!", "OK");

        // Enviarlo a la siguiente pantalla del tutorial
        await Navigation.PushAsync(new HomeTutorialPage());
    }


    private async void btnIniciarSesion_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Próximamente", "La opción de inicio de sesión estará disponible en futuras versiones.", "OK");
    }
}
