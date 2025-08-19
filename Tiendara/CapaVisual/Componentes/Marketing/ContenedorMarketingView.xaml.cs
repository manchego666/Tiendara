using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaVisual.Componentes.Marketing
{
    public partial class ContenedorMarketingView : ContentView
    {
        // ===== Tamaño del panel =====
        public static readonly BindableProperty PanelHeightProperty =
            BindableProperty.Create(nameof(PanelHeight), typeof(double),
                typeof(ContenedorMarketingView), 520.0,
                propertyChanged: (b, o, n) => ((ContenedorMarketingView)b).Panel.HeightRequest = (double)n);
        public double PanelHeight { get => (double)GetValue(PanelHeightProperty); set => SetValue(PanelHeightProperty, value); }

        // ===== Íconos =====
        public static readonly BindableProperty IconCommentProperty =
            BindableProperty.Create(nameof(IconComment), typeof(string), typeof(ContenedorMarketingView), "ic_comment_idle.png");
        public static readonly BindableProperty IconMessageProperty =
            BindableProperty.Create(nameof(IconMessage), typeof(string), typeof(ContenedorMarketingView), "ic_contact_idle.png");
        public static readonly BindableProperty IconProfileProperty =
            BindableProperty.Create(nameof(IconProfile), typeof(string), typeof(ContenedorMarketingView), "ic_profile_idle.png");

        public static readonly BindableProperty IconCommentPressedProperty =
            BindableProperty.Create(nameof(IconCommentPressed), typeof(string), typeof(ContenedorMarketingView), "ic_comment_idle.png");
        public static readonly BindableProperty IconMessagePressedProperty =
            BindableProperty.Create(nameof(IconMessagePressed), typeof(string), typeof(ContenedorMarketingView), "ic_contact_idle.png");
        public static readonly BindableProperty IconProfilePressedProperty =
            BindableProperty.Create(nameof(IconProfilePressed), typeof(string), typeof(ContenedorMarketingView), "ic_profile_idle.png");

        public string IconComment { get => (string)GetValue(IconCommentProperty); set => SetValue(IconCommentProperty, value); }
        public string IconMessage { get => (string)GetValue(IconMessageProperty); set => SetValue(IconMessageProperty, value); }
        public string IconProfile { get => (string)GetValue(IconProfileProperty); set => SetValue(IconProfileProperty, value); }

        public string IconCommentPressed { get => (string)GetValue(IconCommentPressedProperty); set => SetValue(IconCommentPressedProperty, value); }
        public string IconMessagePressed { get => (string)GetValue(IconMessagePressedProperty); set => SetValue(IconMessagePressedProperty, value); }
        public string IconProfilePressed { get => (string)GetValue(IconProfilePressedProperty); set => SetValue(IconProfilePressedProperty, value); }

        // ===== Eventos hacia la Page =====
        public event EventHandler<ItemVm>? CommentRequested;
        public event EventHandler<ItemVm>? ContactRequested;
        public event EventHandler<ItemVm>? ProfileRequested;

        private void RaiseIfItem(object? sender, EventHandler<ItemVm>? ev)
        {
            if (sender is BindableObject bo && bo.BindingContext is ItemVm item)
                ev?.Invoke(this, item);
        }
        private void OnCommentClicked(object? s, EventArgs e) => RaiseIfItem(s, CommentRequested);
        private void OnContactClicked(object? s, EventArgs e) => RaiseIfItem(s, ContactRequested);
        private void OnProfileClicked(object? s, EventArgs e) => RaiseIfItem(s, ProfileRequested);

        // ===== VM simple =====
        private class FeedVm { public ObservableCollection<ItemVm> Items { get; } = new(); }
        private readonly FeedVm _vm = new();

        public class ItemVm
        {
            public Guid Id { get; init; }
            public string AutorLinea { get; init; } = "";
            public string Texto { get; init; } = "";
            public string? ImagenPath { get; init; }
            public bool TieneImagen => !string.IsNullOrWhiteSpace(ImagenPath);
            public DateTime CreadoEn { get; init; }

            public string TiempoRelativo
            {
                get
                {
                    var t = CreadoEn.ToLocalTime();
                    var diff = DateTime.Now - t;
                    if (diff.TotalSeconds < 60) return "hace un momento";
                    if (diff.TotalMinutes < 60) return $"hace {Math.Floor(diff.TotalMinutes)} min";
                    if (diff.TotalHours < 24) return $"hace {Math.Floor(diff.TotalHours)} h";
                    return $"{t:dd MMM yyyy}";
                }
            }

            public static ItemVm From(Publicacion p)
            {
                var autor = p.EsTienda
                    ? (string.IsNullOrWhiteSpace(p.TiendaNombre) ? "Tienda" : p.TiendaNombre)
                    : (string.IsNullOrWhiteSpace(p.AutorNombre) ? "Usuario" : p.AutorNombre);

                return new ItemVm
                {
                    Id = p.Id,
                    AutorLinea = autor,
                    Texto = p.Texto ?? "",
                    ImagenPath = p.ImagenPath,
                    CreadoEn = p.CreadoEn
                };
            }
        }

        private static readonly System.Collections.Generic.List<Publicacion> _store = new();

        public static readonly BindableProperty SelectedTypeProperty =
            BindableProperty.Create(nameof(SelectedType), typeof(PublicationType),
                typeof(ContenedorMarketingView), PublicationType.Noticias,
                propertyChanged: (b, o, n) => ((ContenedorMarketingView)b).OnFilterChanged());
        public PublicationType SelectedType
        {
            get => (PublicationType)GetValue(SelectedTypeProperty);
            set => SetValue(SelectedTypeProperty, value);
        }

        public static readonly BindableProperty SelectedRegionProperty =
            BindableProperty.Create(nameof(SelectedRegion), typeof(Ubicacion),
                typeof(ContenedorMarketingView), new Ubicacion(),
                propertyChanged: (b, o, n) => ((ContenedorMarketingView)b).OnFilterChanged());
        public Ubicacion SelectedRegion
        {
            get => (Ubicacion)GetValue(SelectedRegionProperty);
            set => SetValue(SelectedRegionProperty, value);
        }

        // Para ciclar tipos con tap/swipe
        private static readonly PublicationType[] _types = Enum.GetValues(typeof(PublicationType))
                                                               .Cast<PublicationType>()
                                                               .ToArray();

        public ContenedorMarketingView()
        {
            InitializeComponent();
            BindingContext = _vm;

            // Gestos
            var tap = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
            tap.Tapped += (_, __) => CycleType(+1);
            lblTipoInterno.GestureRecognizers.Add(tap);

            var swipeLeft = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
            swipeLeft.Swiped += (_, __) => CycleType(+1);
            var swipeRight = new SwipeGestureRecognizer { Direction = SwipeDirection.Right };
            swipeRight.Swiped += (_, __) => CycleType(-1);
            Panel.GestureRecognizers.Add(swipeLeft);
            Panel.GestureRecognizers.Add(swipeRight);
        }

        public void SeedIfEmpty()
        {
            if (_store.Count == 0)
            {
                string[] textos = { "Próximamente…", "Evento en puerta", "Actualización en camino", "Nuevas funciones", "Oferta limitada" };

                foreach (PublicationType t in Enum.GetValues(typeof(PublicationType)))
                    for (int i = 0; i < 5; i++)
                        _store.Add(new Publicacion
                        {
                            EsTienda = false,
                            AutorId = Guid.NewGuid(),
                            AutorNombre = "Tiendara Oficial",
                            Texto = textos[i % textos.Length],
                            ImagenPath = null,
                            Type = t,
                            Location = new Ubicacion { Country = "MX", State = "Sinaloa", City = "Culiacán" },
                            Estado = "Publicado",
                            CreadoEn = DateTime.UtcNow.AddMinutes(-5 * (i + 1))
                        });
            }

            OnFilterChanged(); // repinta siempre
        }

        private void CycleType(int dir)
        {
            var idx = Array.IndexOf(_types, SelectedType);
            idx = (idx + dir + _types.Length) % _types.Length;
            SelectedType = _types[idx];
        }

        private bool MatchRegion(Publicacion p, Ubicacion r)
        {
            if (!string.IsNullOrWhiteSpace(r.City))
                return string.Equals(p.Location.City, r.City, StringComparison.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(r.State))
                return string.Equals(p.Location.State, r.State, StringComparison.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(r.Country))
                return string.Equals(p.Location.Country, r.Country, StringComparison.OrdinalIgnoreCase);
            return true;
        }

        private static string TipoToLabel(PublicationType t) => t switch
        {
            PublicationType.Noticias => "Noticias",
            PublicationType.Empleos => "Empleos",
            PublicationType.Promociones => "Promociones",
            PublicationType.VentasRapidas => "Ventas rápidas",
            PublicationType.Servicios => "Servicios",
            PublicationType.Ofertas => "Ofertas",
            PublicationType.Hot => "HOT",
            _ => t.ToString()
        };

        private void OnFilterChanged()
        {
            lblTipoInterno.Text = TipoToLabel(SelectedType);

            _vm.Items.Clear();
            var region = SelectedRegion ?? new Ubicacion();
            var data = _store
                .Where(p => p.Type == SelectedType && MatchRegion(p, region))
                .OrderByDescending(p => p.CreadoEn)
                .Take(200)
                .Select(ItemVm.From);

            foreach (var it in data) _vm.Items.Add(it);
        }
    }
}
