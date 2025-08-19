using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;

namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class ChatThreadPage : ContentPage
    {
        public class MessageVm
        {
            public bool IsMe { get; set; }
            public string Text { get; set; } = "";
            public string Time { get; set; } = "";
        }

        public class Vm { public ObservableCollection<MessageVm> Messages { get; } = new(); }

        readonly Vm _vm = new();
        readonly string _title;

        public ChatThreadPage(string title)
        {
            InitializeComponent();
            _title = title;
            lblTitle.Text = title;

            BindingContext = _vm;

            nav.Activo = "chat";
            nav.HomeClicked += async (_, __) => await Navigation.PopToRootAsync();
            nav.WorldClicked += async (_, __) => await Navigation.PushAsync(new MapPage());
            nav.ChatClicked += (_, __) => { };
            nav.BellClicked += async (_, __) => await DisplayAlert("ZDEV", "Notificaciones (en desarrollo).", "OK");

            btnMenu.Clicked += async (_, __) => await menuLateral.ToggleMenu();

            Seed();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            nav.Activo = "chat";
        }

        void Seed()
        {
            _vm.Messages.Add(new MessageVm { IsMe = false, Text = "Hola! ¿A qué hora abren hoy?", Time = "10:19" });
            _vm.Messages.Add(new MessageVm { IsMe = true, Text = "¡Hola! De 9am a 8pm 😊", Time = "10:20" });
            _vm.Messages.Add(new MessageVm { IsMe = false, Text = "Perfecto, gracias!", Time = "10:21" });
            if (_vm.Messages.Count > 0)
                msgs.ScrollTo(_vm.Messages.Count - 1, position: ScrollToPosition.End, animate: false);
        }

        async void OnSend(object? sender, EventArgs e)
        {
            var t = txt.Text?.Trim();
            if (string.IsNullOrEmpty(t)) return;
            txt.Text = "";

            var now = DateTime.Now.ToString("HH:mm");
            _vm.Messages.Add(new MessageVm { IsMe = true, Text = t, Time = now });

            await System.Threading.Tasks.Task.Delay(300);
            _vm.Messages.Add(new MessageVm { IsMe = false, Text = "Recibido 👍", Time = DateTime.Now.ToString("HH:mm") });

            if (_vm.Messages.Count > 0)
                msgs.ScrollTo(_vm.Messages.Count - 1, position: ScrollToPosition.End, animate: true);
        }

        async void OnOpenProfile(object? sender, EventArgs e)
        {
            await DisplayAlert("Perfil", $"Abrir perfil de: {_title}", "OK");
        }
    }
}
