// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Clase base para todas las entidades SQL que no tienen otra herencia.
// Fecha: 2025-08-19
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tiendara.CapaContratos;

namespace Tiendara.CapaSql.Infra;

public class FileSystemMediaStorage : IMediaStorage
{
    private readonly string _root; // un ejemplo ZDEV "C:\\TiendaraMedia"
    public FileSystemMediaStorage(string root) => _root = root;

    public async Task<string> SaveAsync(string fileName, Stream content, string contentType, CancellationToken ct = default)
    {
        Directory.CreateDirectory(_root);
        var safe = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var path = Path.Combine(_root, safe);
        using var fs = File.Create(path);
        await content.CopyToAsync(fs, ct);

        // Devuelve ruta; en producción podría ser una URL pública de CDN/Blob   - ZDEV2025 -
        return path; // o $"file:///{path.Replace("\\", "/")}"
    }

    public Task DeleteAsync(string url, CancellationToken ct = default)
    {
        if (File.Exists(url)) File.Delete(url);
        return Task.CompletedTask;
    }
}
