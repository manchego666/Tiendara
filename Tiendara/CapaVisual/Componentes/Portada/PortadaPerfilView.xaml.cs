using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tiendara.CapaSql.Conexion;

namespace Tiendara.CapaVisual.Componentes.Portada
{
    public enum PortadaModo { Usuario, Tienda }

    public partial class PortadaPerfilView : ContentView
    {
        public PortadaPerfilView()
        {
            InitializeComponent();
            ActualizarUI();

            // Glow rojo para la marquesina
            lblMarquee.Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Color.FromArgb("#FF2A2A")),
                Radius = 14,
                Opacity = 0.95f,
                Offset = new Point(0, 0)
            };
        }

        // =================== Bindables ===================
        public static readonly BindableProperty ModoProperty =
            BindableProperty.Create(nameof(Modo), typeof(PortadaModo), typeof(PortadaPerfilView),
                PortadaModo.Usuario, propertyChanged: (b, o, n) => ((PortadaPerfilView)b).ActualizarUI());
        public PortadaModo Modo { get => (PortadaModo)GetValue(ModoProperty); set => SetValue(ModoProperty, value); }

        public static readonly BindableProperty TituloProperty =
            BindableProperty.Create(nameof(Titulo), typeof(string), typeof(PortadaPerfilView),
                "Tiendara Pro", propertyChanged: (b, o, n) => ((PortadaPerfilView)b).lblTitulo.Text = (string)n);
        public string Titulo { get => (string)GetValue(TituloProperty); set => SetValue(TituloProperty, value); }

        public static readonly BindableProperty SubtituloProperty =
            BindableProperty.Create(nameof(Subtitulo), typeof(string), typeof(PortadaPerfilView),
                "Dueño", propertyChanged: (b, o, n) => ((PortadaPerfilView)b).lblSubtitulo.Text = (string)n);
        public string Subtitulo { get => (string)GetValue(SubtituloProperty); set => SetValue(SubtituloProperty, value); }

        public static readonly BindableProperty TemaIdProperty =
            BindableProperty.Create(nameof(TemaId), typeof(string), typeof(PortadaPerfilView),
                "Espacio", propertyChanged: (b, o, n) => ((PortadaPerfilView)b).AplicarTema((string)n));
        public string TemaId { get => (string)GetValue(TemaIdProperty); set => SetValue(TemaIdProperty, value); }

        public static readonly BindableProperty FotoPathProperty =
            BindableProperty.Create(nameof(FotoPath), typeof(string), typeof(PortadaPerfilView),
                default(string), propertyChanged: (b, o, n) => ((PortadaPerfilView)b).ActualizarFoto((string?)n));
        public string? FotoPath { get => (string?)GetValue(FotoPathProperty); set => SetValue(FotoPathProperty, value); }

        public static readonly BindableProperty EmpleadosCountProperty =
            BindableProperty.Create(nameof(EmpleadosCount), typeof(int), typeof(PortadaPerfilView),
                0, propertyChanged: (b, o, n) => ((PortadaPerfilView)b).ActualizarContadores());
        public int EmpleadosCount { get => (int)GetValue(EmpleadosCountProperty); set => SetValue(EmpleadosCountProperty, value); }

        public static readonly BindableProperty TiendasCountProperty =
            BindableProperty.Create(nameof(TiendasCount), typeof(int), typeof(PortadaPerfilView),
                0, propertyChanged: (b, o, n) => ((PortadaPerfilView)b).ActualizarContadores());
        public int TiendasCount { get => (int)GetValue(TiendasCountProperty); set => SetValue(TiendasCountProperty, value); }

        public static readonly BindableProperty NoticiasCountProperty =
            BindableProperty.Create(nameof(NoticiasCount), typeof(int), typeof(PortadaPerfilView),
                0, propertyChanged: (b, o, n) => ((PortadaPerfilView)b).ActualizarContadores());
        public int NoticiasCount { get => (int)GetValue(NoticiasCountProperty); set => SetValue(NoticiasCountProperty, value); }

        // =================== Eventos a la Page ===================
        public event EventHandler? VerFotoSolicitado;
        public event EventHandler? CambiarFotoSolicitado;
        public event EventHandler? EditarDatosClicked;
        public event EventHandler? EditarTemasClicked;

        // =================== UI Helpers ===================
        private void ActualizarUI()
        {
            ActualizarContadores();
            AplicarTema(TemaId);
            ActualizarFoto(FotoPath);
        }



        private void ActualizarContadores()
        {
            if (Modo == PortadaModo.Tienda)
            {
                // Tienda: Sucursales + Empleados
                lblContadorA.Text = $"Sucursales: {TiendasCount}";
                lblContadorB.Text = $"Empleados: {EmpleadosCount}";
            }
            else
            {
                // Usuario: Tiendas + Publicaciones
                lblContadorA.Text = $"Tiendas: {TiendasCount}";
                lblContadorB.Text = $"Publicaciones: {NoticiasCount}";
            }
        }

        private void AplicarTema(string? temaId)
        {
            // Por ahora un único fondo “espacio”
            imgTema.Source = "portada_espacio.png";
        }

        private void ActualizarFoto(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                imgAvatar.Source = "avatar_default.png";
                return;
            }

            string fullPath = Modo == PortadaModo.Usuario
                ? Path.Combine(ConfiguracionSql.UsuariosAvatares, fileName)
                : Path.Combine(ConfiguracionSql.NegociosLogos, fileName);

            imgAvatar.Source = File.Exists(fullPath)
                ? ImageSource.FromFile(fullPath)
                : (Modo == PortadaModo.Usuario ? "avatar_default.png" : "logo_default.png");
        }



        // Mantener 16:9 a todo el ancho del hero
        private void OnHeroSizeChanged(object? sender, EventArgs e)
        {
            if (hero.Width > 0)
                hero.HeightRequest = hero.Width * 9.0 / 16.0;
        }

        // Gestos internos
        private void OnAvatarTapped(object? sender, EventArgs e) => VerFotoSolicitado?.Invoke(this, EventArgs.Empty);
        private void OnChipFotoTapped(object? sender, EventArgs e) => CambiarFotoSolicitado?.Invoke(this, EventArgs.Empty);
        private void OnEditarDatosClicked(object? sender, EventArgs e) => EditarDatosClicked?.Invoke(this, EventArgs.Empty);
        private void OnEditarTemasClicked(object? sender, EventArgs e) => EditarTemasClicked?.Invoke(this, EventArgs.Empty);

        // =================== Marquesina ===================
        private const double MarqueeSpeedPxPerSec = 90;
        private const string MarqueeText = "ZDEV · 2025 · TIENDARA+";
        private CancellationTokenSource? _marqueeCts;
        private bool _marqueeRunning;

        private void OnFranjaSizeChanged(object? sender, EventArgs e)
        {
            marqueeClip.Rect = new Rect(0, 0, franja.Width, franja.Height);

            // tamaño de texto proporcional (con límites)
            lblMarquee.FontSize = Math.Clamp(franja.Height * 0.50, 18, 28);

            if (!_marqueeRunning && franja.Width > 0)
                _ = RunMarqueeLoop();
        }

        private async Task RunMarqueeLoop()
        {
            if (_marqueeRunning) return;
            _marqueeRunning = true;

            _marqueeCts?.Cancel();
            _marqueeCts = new CancellationTokenSource();
            var ct = _marqueeCts.Token;

            await Task.Delay(120, ct);

            while (!ct.IsCancellationRequested)
            {
                lblMarquee.Text = MarqueeText;
                lblMarquee.InvalidateMeasure();

                double hostW = franja.Width;
                if (hostW <= 0) { await Task.Delay(400, ct); continue; }

                var size = lblMarquee.Measure(double.PositiveInfinity, double.PositiveInfinity);
                double labelW = Math.Max(1, size.Width);   // .NET 9: Size.Width

                lblMarquee.TranslationX = hostW;
                lblMarquee.TranslationY = 0;

                double distance = hostW + labelW;
                uint durMs = (uint)Math.Max(1200, distance / MarqueeSpeedPxPerSec * 1000.0);

                await lblMarquee.TranslateTo(-labelW, 0, durMs, Easing.Linear);
                // loop continuo
            }
        }

        // Llamado opcional para parar desde la Page
        public void StopMarquee()
        {
            try { _marqueeCts?.Cancel(); } catch { }
            _marqueeRunning = false;
        }
    }
}
