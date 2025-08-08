using System;
using Microsoft.Maui.Controls;
using Tiendara.CapaVisual;

namespace Tiendara;

public partial class HomeTutorialPage : ContentPage

{
    private int paso = 0;
    private readonly string[] mensajes = new string[]
    {
        "¡Bienvenido al siguiente paso! Aquí podrás registrar tu tienda.",
        "Podrás especificar el tipo. Ya sea Tienda, Negocio de comida rápida, Fruterías, Carnicerías, etc.",
        "Podrás tener 1 tienda en esta cuenta, pero con la versión VIP podrás tener una versión más privilegiada con más derechos ¡y a un precio muy barato!"
    };

    public HomeTutorialPage()
    {
        InitializeComponent();

        // Activar toque para avanzar
        var tap = new TapGestureRecognizer();
        tap.Tapped += AvanzarTutorial;
        frameExplicacion.GestureRecognizers.Add(tap);
    }

    private void AvanzarTutorial(object? sender, EventArgs e)
    {
        paso++;

        if (paso < mensajes.Length)
        {
            lblExplicacion.Text = mensajes[paso];
        }
        else
        {
            btnRegistrarTienda.IsVisible = true;
        }
    }

    private async void btnRegistrarTienda_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistroTiendaPage());
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Animación opcional si lo quieres chido
        await Task.Delay(300);
        _ = Parpadear(lblExplicacion);
    }

    private async Task Parpadear(Label label)
    {
        while (paso < mensajes.Length)
        {
            await label.FadeTo(0.3, 400);
            await label.FadeTo(1, 400);
        }
    }


}
