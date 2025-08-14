using System;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Tiendara.CapaDatos.Repos;
using Tiendara.CapaLogica.Servicios;

namespace Tiendara.CapaVisual.Componentes.Login;

public partial class MonedaLoginView : ContentView
{
    private readonly AuthLocalService _auth = new(new UsuarioRepo());

    const uint FlipDurationMs = 2000; // 2.0s
    const int FlipTurns = 6;    // 6 vueltas
    bool _animBusy;

    public MonedaLoginView()
    {
        InitializeComponent();

        // Clip circular y shimmer al tamaño de la moneda
        Coin.SizeChanged += (_, __) =>
        {
            var cx = Coin.Width / 2;
            var cy = Coin.Height / 2;
            CoinClip.Center = new Point(cx, cy);
            CoinClip.RadiusX = cx;
            CoinClip.RadiusY = cy;

            Shimmer.WidthRequest = Coin.Width;
            Shimmer.HeightRequest = Coin.Height;
        };

        StartIdleShimmer();
    }

    // ======== Handlers que XAML necesita ========
    private void OnToggleLoginPass(object? s, EventArgs e) => tbLoginPass.IsPassword = !tbLoginPass.IsPassword;
    private void OnToggleRegPass(object? s, EventArgs e) => tbRegPass.IsPassword = !tbRegPass.IsPassword;

    private async void OnFlipToRegister(object? s, EventArgs e) => await FlipAsync(toRegister: true);

    private async void OnLoginClicked(object? s, EventArgs e)
    {
        var email = (tbLoginEmail.Text ?? "").Trim();
        var pass = tbLoginPass.Text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
        { await Alert("Faltan datos", "Ingresa email y contraseña."); return; }

        var (ok, err, u) = await _auth.LoginAsync(email, pass);
        if (!ok)
        { await Alert("Ups", err ?? "Usuario o contraseña incorrectos."); return; }

        await Shell.Current.GoToAsync("//home");
    }

    private async void OnRegisterClicked(object? s, EventArgs e)
    {
        var nombre = (tbRegNombre.Text ?? "").Trim();
        var email = (tbRegEmail.Text ?? "").Trim();
        var pass = tbRegPass.Text ?? "";

        if (string.IsNullOrWhiteSpace(nombre))
        { await Alert("Faltan datos", "Escribe tu nombre."); return; }
        if (string.IsNullOrWhiteSpace(email))
        { await Alert("Faltan datos", "Escribe tu correo."); return; }
        if (pass.Length < 6)
        { await Alert("Contraseña", "Debe tener al menos 6 caracteres."); return; }

        var (ok, err, u) = await _auth.RegistrarAsync(nombre, email, pass);
        if (!ok)
        { await Alert("Atención", err ?? "No se pudo crear la cuenta."); return; }

        await Alert("¡Registro exitoso!", "Tu cuenta fue creada correctamente.");

        // Prellenar y volver al login automáticamente
        tbLoginEmail.Text = email;
        await FlipAsync(toRegister: false);
    }

    // ======== Animaciones ========
    private async Task FlipAsync(bool toRegister)
    {
        if (_animBusy) return;
        _animBusy = true;

        FrontPanel.IsVisible = false;
        BackPanel.IsVisible = false;

        double target = Coin.RotationY + 360 * FlipTurns;
        await Coin.RotateYTo(target, FlipDurationMs, Easing.Linear);

        if (toRegister) BackPanel.IsVisible = true;
        else FrontPanel.IsVisible = true;

        Coin.RotationY = 0;
        _animBusy = false;
    }

    void StartIdleShimmer()
    {
        Device.StartTimer(TimeSpan.FromSeconds(3.2), () =>
        {
            _ = RunShimmerOnce();
            return true;
        });
    }

    async Task RunShimmerOnce()
    {
        if (Coin.Width <= 0) return;

        Shimmer.IsVisible = true;
        Shimmer.Opacity = 0.0;
        Shimmer.TranslationX = -Coin.Width;

        await Shimmer.FadeTo(0.35, 120, Easing.CubicIn);
        await Shimmer.TranslateTo(Coin.Width, 0, 1600u, Easing.CubicInOut);
        await Shimmer.FadeTo(0.0, 160, Easing.CubicOut);
    }

    static Task Alert(string title, string msg)
        => Application.Current?.MainPage?.DisplayAlert(title, msg, "OK") ?? Task.CompletedTask;
}
