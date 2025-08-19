using System;
using System.Globalization;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Tiendara.CapaDatos.Entidades; // NavTab

namespace Tiendara.CapaVisual.Componentes.Navegacion
{
    public partial class BarraNavegacionInferior : ContentView
    {
        public BarraNavegacionInferior()
        {
            InitializeComponent();
            ApplyBadges();
            ApplyVisual();
        }

        // Compat: prop string “Activo” -> mapea a SelectedTab (acepta "home","world","chat","bell","none" o 0..3/-1)
        public static readonly BindableProperty ActivoProperty =
            BindableProperty.Create(nameof(Activo), typeof(string), typeof(BarraNavegacionInferior),
                default(string), propertyChanged: (b, o, n) => ((BarraNavegacionInferior)b).MapActivoToSelected(n as string));

        public string? Activo { get => (string?)GetValue(ActivoProperty); set => SetValue(ActivoProperty, value); }

        // Selección real
        public static readonly BindableProperty SelectedTabProperty =
            BindableProperty.Create(nameof(SelectedTab), typeof(NavTab), typeof(BarraNavegacionInferior),
                NavTab.None, propertyChanged: (b, o, n) => ((BarraNavegacionInferior)b).ApplyVisual());

        public NavTab SelectedTab { get => (NavTab)GetValue(SelectedTabProperty); set => SetValue(SelectedTabProperty, value); }

        // Comandos opcionales
        public static readonly BindableProperty HomeCommandProperty = BindableProperty.Create(nameof(HomeCommand), typeof(ICommand), typeof(BarraNavegacionInferior));
        public static readonly BindableProperty WorldCommandProperty = BindableProperty.Create(nameof(WorldCommand), typeof(ICommand), typeof(BarraNavegacionInferior));
        public static readonly BindableProperty ChatCommandProperty = BindableProperty.Create(nameof(ChatCommand), typeof(ICommand), typeof(BarraNavegacionInferior));
        public static readonly BindableProperty BellCommandProperty = BindableProperty.Create(nameof(BellCommand), typeof(ICommand), typeof(BarraNavegacionInferior));

        public ICommand? HomeCommand { get => (ICommand?)GetValue(HomeCommandProperty); set => SetValue(HomeCommandProperty, value); }
        public ICommand? WorldCommand { get => (ICommand?)GetValue(WorldCommandProperty); set => SetValue(WorldCommandProperty, value); }
        public ICommand? ChatCommand { get => (ICommand?)GetValue(ChatCommandProperty); set => SetValue(ChatCommandProperty, value); }
        public ICommand? BellCommand { get => (ICommand?)GetValue(BellCommandProperty); set => SetValue(BellCommandProperty, value); }

        // Eventos
        public event EventHandler? HomeClicked;
        public event EventHandler? WorldClicked;
        public event EventHandler? ChatClicked;
        public event EventHandler? BellClicked;

        // Badges
        public static readonly BindableProperty HomeBadgeProperty = BindableProperty.Create(nameof(HomeBadge), typeof(int), typeof(BarraNavegacionInferior), 0, propertyChanged: OnBadgeChanged);
        public static readonly BindableProperty WorldBadgeProperty = BindableProperty.Create(nameof(WorldBadge), typeof(int), typeof(BarraNavegacionInferior), 0, propertyChanged: OnBadgeChanged);
        public static readonly BindableProperty ChatBadgeProperty = BindableProperty.Create(nameof(ChatBadge), typeof(int), typeof(BarraNavegacionInferior), 0, propertyChanged: OnBadgeChanged);
        public static readonly BindableProperty BellBadgeProperty = BindableProperty.Create(nameof(BellBadge), typeof(int), typeof(BarraNavegacionInferior), 0, propertyChanged: OnBadgeChanged);

        public int HomeBadge { get => (int)GetValue(HomeBadgeProperty); set => SetValue(HomeBadgeProperty, value); }
        public int WorldBadge { get => (int)GetValue(WorldBadgeProperty); set => SetValue(WorldBadgeProperty, value); }
        public int ChatBadge { get => (int)GetValue(ChatBadgeProperty); set => SetValue(ChatBadgeProperty, value); }
        public int BellBadge { get => (int)GetValue(BellBadgeProperty); set => SetValue(BellBadgeProperty, value); }

        static void OnBadgeChanged(BindableObject b, object o, object n) => ((BarraNavegacionInferior)b).ApplyBadges();

        // Preferencias
        public static readonly BindableProperty UseHapticsProperty = BindableProperty.Create(nameof(UseHaptics), typeof(bool), typeof(BarraNavegacionInferior), true);
        public bool UseHaptics { get => (bool)GetValue(UseHapticsProperty); set => SetValue(UseHapticsProperty, value); }

        public static readonly BindableProperty PillMaxWidthProperty = BindableProperty.Create(nameof(PillMaxWidth), typeof(double), typeof(BarraNavegacionInferior), 520d);
        public double PillMaxWidth { get => (double)GetValue(PillMaxWidthProperty); set => SetValue(PillMaxWidthProperty, value); }

        // ===== Taps =====
        async void OnHomeTapped(object? s, EventArgs e) { Tap(NavTab.Home, HomeCommand, HomeClicked); await PulseAsync(imgHome); }
        async void OnWorldTapped(object? s, EventArgs e) { Tap(NavTab.World, WorldCommand, WorldClicked); await PulseAsync(imgWorld); }
        async void OnChatTapped(object? s, EventArgs e) { Tap(NavTab.Chat, ChatCommand, ChatClicked); await PulseAsync(imgChat); }
        async void OnBellTapped(object? s, EventArgs e) { Tap(NavTab.Bell, BellCommand, BellClicked); await PulseAsync(imgBell); }

        void Tap(NavTab tab, ICommand? cmd, EventHandler? evt)
        {
            TryHaptics();
            SelectedTab = tab;
            cmd?.Execute(null);
            evt?.Invoke(this, EventArgs.Empty);
        }

        // ===== Visual =====
        void ApplyVisual()
        {
            // reset
            dotHome.Opacity = dotWorld.Opacity = dotChat.Opacity = dotBell.Opacity = 0;
            imgHome.Source = "ic_home_idle.png";
            imgWorld.Source = "ic_world_idle.png";
            imgChat.Source = "ic_chat_idle.png";
            imgBell.Source = "ic_bell_idle.png";

            if (SelectedTab == NavTab.None) return;

            switch (SelectedTab)
            {
                case NavTab.Home: imgHome.Source = "ic_home_active.png"; dotHome.Opacity = 1; break;
                case NavTab.World: imgWorld.Source = "ic_world_active.png"; dotWorld.Opacity = 1; break;
                case NavTab.Chat: imgChat.Source = "ic_chat_active.png"; dotChat.Opacity = 1; break;
                case NavTab.Bell: imgBell.Source = "ic_bell_active.png"; dotBell.Opacity = 1; break;
            }
        }

        void ApplyBadges()
        {
            SetBadge(badgeHome, lblBadgeHome, HomeBadge);
            SetBadge(badgeWorld, lblBadgeWorld, WorldBadge);
            SetBadge(badgeChat, lblBadgeChat, ChatBadge);
            SetBadge(badgeBell, lblBadgeBell, BellBadge);
        }

        static void SetBadge(Frame badge, Label label, int value)
        {
            if (value <= 0) { badge.IsVisible = false; return; }
            badge.IsVisible = true;
            label.Text = value > 99 ? "99+" : value.ToString();
        }

        // Mapear string "Activo"
        void MapActivoToSelected(string? val)
        {
            if (string.IsNullOrWhiteSpace(val)) { SelectedTab = NavTab.None; return; }
            var s = val.Trim().ToLowerInvariant();

            if (s is "none" or "ninguno" or "null") { SelectedTab = NavTab.None; return; }

            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var idx) && idx >= -1 && idx <= 3)
            { SelectedTab = (NavTab)idx; return; }

            switch (s)
            {
                case "home": case "inicio": SelectedTab = NavTab.Home; return;
                case "world": case "mundo": case "map": case "mapa": SelectedTab = NavTab.World; return;
                case "chat": case "mensajes": SelectedTab = NavTab.Chat; return;
                case "bell": case "notif": case "notificaciones": SelectedTab = NavTab.Bell; return;
                default: SelectedTab = NavTab.None; return;
            }
        }

        static async System.Threading.Tasks.Task PulseAsync(VisualElement v)
        { try { await v.ScaleTo(0.9, 80, Easing.CubicOut); await v.ScaleTo(1.0, 100, Easing.CubicOut); } catch { } }

        void TryHaptics()
        {
            if (!UseHaptics) return;
            try { HapticFeedback.Default?.Perform(HapticFeedbackType.Click); }
            catch
            {
                try
                {
                    if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
                        Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(25));
                }
                catch { }
            }
        }
    }
}
