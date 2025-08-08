using Microsoft.Maui.Controls;
using Microsoft.Maui.Media; // MediaPicker
using System;
using System.IO;
using System.Threading.Tasks;

namespace Tiendara.CapaVisual.PaginasModulo;

public partial class PerfilPage : ContentPage
{
    // Simulación de usuario actual (cuando tenga mi modelo, bindeo real)
    private string _personaId = "USR-001";
    private string? _fotoLocalPath;

    public PerfilPage()
    {
        InitializeComponent();

        // Placeholders visibles
        lblNombre.Text = "Tiendara Pro";
        lblRol.Text = "Dueño";
        lblEmpleados.Text = "Empleados: 0";
        lblTiendas.Text = "Tiendas: 0";
    }

    // Tap en la imagen: ver a pantalla completa con opción de cerrar
    private async void OnAvatarTapped(object sender, EventArgs e)
    {
        var img = new Image
        {
            Aspect = Aspect.AspectFit,
            Source = imgAvatar.Source,
            BackgroundColor = Colors.Black
        };

        var viewer = new ContentPage
        {
            Title = "Foto de perfil",
            BackgroundColor = Colors.Black,
            Content = new Grid { Children = { img } }
        };

        // Cerrar desde toolbar
        viewer.ToolbarItems.Add(new ToolbarItem("Cerrar", null, async () => await Navigation.PopModalAsync()));
        // Cerrar tocando la imagen
        img.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () => await Navigation.PopModalAsync())
        });

        await Navigation.PushModalAsync(new NavigationPage(viewer)
        {
            BarTextColor = Colors.White,
            BarBackgroundColor = Colors.Black
        });
    }

    // Chip 'Foto': elegir cámara o galería
    private async void OnFotoChipTapped(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("Foto de perfil", "Cancelar", null, "Desde cámara", "Desde galería");
        if (action == "Desde cámara")
        {
            await TomarFotoAsync();
        }
        else if (action == "Desde galería")
        {
            await CambiarFotoDesdeGaleriaAsync();
        }
    }

    private async Task TomarFotoAsync()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            if (photo == null) return;
            await GuardarYActualizarAsync(photo);
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Cámara", "Este dispositivo no soporta cámara.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permisos", "Concede permiso de cámara/almacenamiento.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task CambiarFotoDesdeGaleriaAsync()
    {
        try
        {
            var file = await MediaPicker.PickPhotoAsync();
            if (file == null) return;
            await GuardarYActualizarAsync(file);
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Galería", "Este dispositivo no soporta selección de fotos.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permisos", "Concede permiso para acceder a fotos/almacenamiento.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task GuardarYActualizarAsync(FileResult file)
    {
        var dir = FileSystem.AppDataDirectory; // sandbox de la app (no va al repo)
        var filename = $"avatar_{_personaId}.jpg";
        var dest = Path.Combine(dir, filename);

        using (var src = await file.OpenReadAsync())
        using (var dst = File.Create(dest))
            await src.CopyToAsync(dst);

        _fotoLocalPath = dest;

        // Forzar refresco sin caché
        imgAvatar.Source = null;
        await Task.Delay(10);
        imgAvatar.Source = ImageSource.FromStream(() => File.OpenRead(dest));
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Perfil", "Aquí irá 'EditarPerfil' (próximamente).", "OK");
        // Futuro: await Navigation.PushAsync(new EditarPerfilPage());
    }
}
