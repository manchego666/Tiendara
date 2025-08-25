using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Tiendara.CapaContratos;

public sealed class FotoApiHttp : IFotoApi
{
    private readonly IHttpClientFactory _factory;
    public FotoApiHttp(IHttpClientFactory factory) => _factory = factory;

    public Task<string> SubirAvatarAsync(Guid usuarioId, Stream s, string name)
        => PostAsync($"/api/media/usuario/{usuarioId}/avatar", s, name);

    public Task<string> SubirLogoAsync(Guid negocioId, Stream s, string name)
        => PostAsync($"/api/media/negocio/{negocioId}/logo", s, name);

    private async Task<string> PostAsync(string url, Stream stream, string fileName)
    {
        var http = _factory.CreateClient("Api");
        using var content = new MultipartFormDataContent();
        using var file = new StreamContent(stream);
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        file.Headers.ContentType = new MediaTypeHeaderValue(ext == ".png" ? "image/png" : "image/jpeg");
        content.Add(file, "file", fileName);

        var resp = await http.PostAsync(url, content);
        resp.EnsureSuccessStatusCode();

        var dto = await resp.Content.ReadFromJsonAsync<UploadResult>();
        // devuelve URL absoluta combinando BaseAddress + url relativa del server
        return $"{http.BaseAddress!.ToString().TrimEnd('/')}{dto!.url}";
    }

    private sealed record UploadResult(string relative, string url);
}
