using Microsoft.Maui.Storage;

namespace Tiendara.CapaLogica.Infra
{
    public static class BackendConfig
    {
        private const string Key = "BackendBaseUrl";
        private const string DefaultUrl = "http://192.168.1.12:5080";

        public static string BaseUrl
        {
            get => Preferences.Get(Key, DefaultUrl).TrimEnd('/');
            set => Preferences.Set(Key, value.TrimEnd('/'));
        }

        // Acepta URL absoluta o ruta relativa de BD y devuelve URL absoluta lista para Image
        public static string ToAbsoluteMediaUrl(string? pathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(pathOrUrl)) return string.Empty;
            if (pathOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return pathOrUrl;

            var rel = pathOrUrl.Replace("\\", "/").TrimStart('/');
            if (!rel.StartsWith("media/", StringComparison.OrdinalIgnoreCase))
                rel = $"media/{rel}";
            return $"{BaseUrl}/{rel}";
        }
    }
}
