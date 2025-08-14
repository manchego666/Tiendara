using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace Tiendara.CapaVisual.Utils
{
    public static class Permisos
    {
        public static async Task<bool> EnsureFotoAsync()
        {
            var cam = await Permissions.RequestAsync<Permissions.Camera>();
            var photos = await Permissions.RequestAsync<Permissions.Photos>();
            return cam == PermissionStatus.Granted && photos == PermissionStatus.Granted;
        }
    }
}
