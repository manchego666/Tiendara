using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class ChatListPage : ContentPage
    {
        public class ChatRow
        {
            public string Title { get; set; } = "";
            public string Last { get; set; } = "";
            public string Time { get; set; } = "";
            public int Unread { get; set; }
            public string Avatar { get; set; } = "avatar_default.png";
        }

        public class Vm { public ObservableCollection<ChatRow> Items { get; } = new(); }

        readonly Vm _vm = new();

        public ChatListPage()
        {
            InitializeComponent();
            BindingContext = _vm;

            // nav
            nav.Activo = "chat";
            nav.HomeClicked += async (_, __) => await Navigation.PopToRootAsync();
            nav.WorldClicked += async (_, __) => await Navigation.PushAsync(new MapPage());
            nav.ChatClicked += (_, __) => { /* ya aquí */ };
            nav.BellClicked += async (_, __) => await DisplayAlert("ZDEV", "Notificaciones (en desarrollo).", "OK");

            // menú
            btnMenu.Clicked += async (_, __) => await menuLateral.ToggleMenu();

            Seed();
        }

        void Seed()
        {
            _vm.Items.Clear();
            _vm.Items.Add(new ChatRow { Title = "Cliente – Ana", Last = "¿Abren hoy?", Time = "10:21", Unread = 2 });
            _vm.Items.Add(new ChatRow { Title = "Cliente – Luis", Last = "Gracias, nos vemos!", Time = "Ayer", Unread = 0 });
            _vm.Items.Add(new ChatRow { Title = "Proveedor – Panadería", Last = "Entrego a las 8am", Time = "Lun", Unread = 0 });
        }

        async void OnSelect(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection?.FirstOrDefault() is ChatRow row)
            {
                ((CollectionView)sender).SelectedItem = null;
                await Navigation.PushAsync(new ChatThreadPage(row.Title));
            }
        }
    }
}
