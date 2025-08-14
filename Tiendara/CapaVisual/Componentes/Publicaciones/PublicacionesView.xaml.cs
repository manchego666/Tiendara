using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaVisual.Componentes.Publicaciones
{
    public enum PublicacionesModo { Usuario, Tienda }

    public partial class PublicacionesView : ContentView
    {
        // ===== Bindables: identidad del contexto =====
        public static readonly BindableProperty ModoProperty =
            BindableProperty.Create(nameof(Modo), typeof(PublicacionesModo), typeof(PublicacionesView),
                PublicacionesModo.Usuario, propertyChanged: (b, o, n) => ((PublicacionesView)b).Recargar());

        public PublicacionesModo Modo
        {
            get => (PublicacionesModo)GetValue(ModoProperty);
            set => SetValue(ModoProperty, value);
        }

        public static readonly BindableProperty AutorIdProperty =
            BindableProperty.Create(nameof(AutorId), typeof(Guid), typeof(PublicacionesView),
                defaultValueCreator: _ => Guid.NewGuid(), propertyChanged: (b, o, n) => ((PublicacionesView)b).Recargar());
        public Guid AutorId { get => (Guid)GetValue(AutorIdProperty); set => SetValue(AutorIdProperty, value); }

        public static readonly BindableProperty AutorNombreProperty =
            BindableProperty.Create(nameof(AutorNombre), typeof(string), typeof(PublicacionesView),
                "Usuario");
        public string AutorNombre { get => (string)GetValue(AutorNombreProperty); set => SetValue(AutorNombreProperty, value); }

        public static readonly BindableProperty TiendaIdProperty =
            BindableProperty.Create(nameof(TiendaId), typeof(Guid?), typeof(PublicacionesView),
                null, propertyChanged: (b, o, n) => ((PublicacionesView)b).Recargar());
        public Guid? TiendaId { get => (Guid?)GetValue(TiendaIdProperty); set => SetValue(TiendaIdProperty, value); }

        public static readonly BindableProperty TiendaNombreProperty =
            BindableProperty.Create(nameof(TiendaNombre), typeof(string), typeof(PublicacionesView),
                null);
        public string? TiendaNombre { get => (string?)GetValue(TiendaNombreProperty); set => SetValue(TiendaNombreProperty, value); }

        // ===== Eventos hacia la Page (para enganchar navegación/acciones) =====
        public event EventHandler<PublicacionItem>? CommentRequested;
        public event EventHandler<PublicacionItem>? ContactRequested;
        public event EventHandler<PublicacionItem>? ProfileRequested;

        // ===== VM =====
        public ObservableCollection<PublicacionItem> Items { get; } = new();

        private string? _imagenTempPath;

        public PublicacionesView()
        {
            InitializeComponent();
            BindingContext = this;
            Recargar();
        }

        // ===== Composer: adjuntar / quitar / publicar =====
        private async void OnAdjuntarClicked(object sender, EventArgs e)
        {
            var action = await Application.Current.MainPage.DisplayActionSheet(
                "Adjuntar imagen", "Cancelar", null, "Desde cámara", "Desde galería");

            try
            {
                FileResult? fr = null;
                if (action == "Desde cámara") fr = await MediaPicker.CapturePhotoAsync();
                if (action == "Desde galería") fr = await MediaPicker.PickPhotoAsync();
                if (fr == null) return;

                var dir = FileSystem.AppDataDirectory;
                var filename = $"pub_{Guid.NewGuid():N}.jpg";
                var dest = Path.Combine(dir, filename);

                using var src = await fr.OpenReadAsync();
                using var dst = File.Create(dest);
                await src.CopyToAsync(dst);

                _imagenTempPath = dest;
                imgPreview.Source = ImageSource.FromFile(dest);
                panelPreview.IsVisible = true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void OnQuitarImagenClicked(object? sender, EventArgs e)
        {
            _imagenTempPath = null;
            panelPreview.IsVisible = false;
            imgPreview.Source = null;
        }

        private async void OnPublicarClicked(object sender, EventArgs e)
        {
            var texto = (txtPost.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(texto) && string.IsNullOrWhiteSpace(_imagenTempPath))
            {
                await Application.Current.MainPage.DisplayAlert("Publicación", "Escribe algo o adjunta una imagen.", "OK");
                return;
            }

            var pub = new Publicacion
            {
                AutorId = AutorId,
                EsTienda = (Modo == PublicacionesModo.Tienda),
                TiendaId = (Modo == PublicacionesModo.Tienda ? (TiendaId ?? Guid.Empty) : null),
                AutorNombre = AutorNombre,
                TiendaNombre = TiendaNombre,
                Texto = texto,
                ImagenPath = _imagenTempPath,
                Estado = "Publicado",
                CreadoEn = DateTime.UtcNow
            };

            // Persistir en memoria (sandbox actual)
            PublicacionesSessionStore.Publicaciones.Add(pub);

            // Pintar arriba
            Items.Insert(0, PublicacionItem.From(pub));

            // Limpiar composer
            txtPost.Text = string.Empty;
            OnQuitarImagenClicked(this, EventArgs.Empty);
        }

        // ===== Feed =====
        private void Recargar()
        {
            Items.Clear();

            var data = PublicacionesSessionStore.Publicaciones
                .Where(p => Modo == PublicacionesModo.Usuario
                                ? (!p.EsTienda && p.AutorId == AutorId)
                                : (p.EsTienda && p.TiendaId == TiendaId))
                .OrderByDescending(p => p.CreadoEn)
                .Take(200)
                .Select(PublicacionItem.From);

            foreach (var it in data) Items.Add(it);
        }

        // ===== Acciones por card =====
        private void RaiseIfItem(object? sender, EventHandler<PublicacionItem>? ev)
        {
            if (sender is BindableObject bo && bo.BindingContext is PublicacionItem item)
                ev?.Invoke(this, item);
        }

        private void OnCommentClicked(object? s, EventArgs e) => RaiseIfItem(s, CommentRequested);
        private void OnContactClicked(object? s, EventArgs e) => RaiseIfItem(s, ContactRequested);
        private void OnProfileClicked(object? s, EventArgs e) => RaiseIfItem(s, ProfileRequested);
    }

    // ===== VM para ítems (igual que el tuyo, lo dejo completo por claridad) =====
    public sealed class PublicacionItem
    {
        public Guid Id { get; init; }
        public string AutorLinea { get; init; } = "";
        public string ChipOrigen { get; init; } = "";
        public string Texto { get; init; } = "";
        public string? ImagenPath { get; init; }
        public bool TieneImagen => !string.IsNullOrWhiteSpace(ImagenPath);
        public string TiempoRelativo { get; init; } = "";

        public static PublicacionItem From(Publicacion p)
        {
            var autor = p.EsTienda
                ? (string.IsNullOrWhiteSpace(p.TiendaNombre) ? "Tienda" : p.TiendaNombre)
                : (string.IsNullOrWhiteSpace(p.AutorNombre) ? "Usuario" : p.AutorNombre);

            return new PublicacionItem
            {
                Id = p.Id,
                AutorLinea = autor,
                ChipOrigen = p.EsTienda ? "Tienda" : "Usuario",
                Texto = p.Texto ?? "",
                ImagenPath = p.ImagenPath,
                TiempoRelativo = RelTime(p.CreadoEn)
            };
        }

        private static string RelTime(DateTime t)
        {
            var diff = DateTime.UtcNow - t;
            if (diff.TotalSeconds < 60) return "hace un momento";
            if (diff.TotalMinutes < 60) return $"hace {Math.Floor(diff.TotalMinutes)} min";
            if (diff.TotalHours < 24) return $"hace {Math.Floor(diff.TotalHours)} h";
            return $"{t:dd MMM yyyy}";
        }
    }

    // ===== Store en memoria (igual al tuyo) =====
    internal static class PublicacionesSessionStore
    {
        public static readonly System.Collections.Generic.List<Publicacion> Publicaciones = new();
    }
}
