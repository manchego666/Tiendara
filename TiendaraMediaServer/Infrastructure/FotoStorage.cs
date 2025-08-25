using TiendaraMediaServer.Contracts;
using TiendaraMediaServer.Infrastructure;

namespace TiendaraMediaServer.Infrastructure;

public sealed class FotoStorage : IFotoStorage
{
    private readonly string _root;
    private readonly string _basePath;

    public FotoStorage(MediaOptions opt)
    {
        _root = opt.Root;
        _basePath = opt.BasePath.TrimEnd('/');
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(MediaArea area, Guid id, Stream file, string ext)
    {
        ext = ext.StartsWith(".") ? ext : "." + ext;
        var subdir = area == MediaArea.UsuarioAvatar ? "Usuarios/Avatares" : "Negocios/Logos";
        var name = area == MediaArea.UsuarioAvatar ? $"perfil_{id:N}{ext}" : $"negocio_{id:N}{ext}";
        var dir = Path.Combine(_root, subdir.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(dir);

        var abs = Path.Combine(dir, name);
        using var fs = File.Create(abs);
        await file.CopyToAsync(fs);

        return $"{subdir}/{name}".Replace('\\', '/'); // relativo para BD
    }

    public Task DeleteIfExistsAsync(string rel)
    {
        var abs = Path.Combine(_root, rel.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(abs)) File.Delete(abs);
        return Task.CompletedTask;
    }

    public bool Exists(string rel)
    {
        var abs = Path.Combine(_root, rel.Replace('/', Path.DirectorySeparatorChar));
        return File.Exists(abs);
    }

    public string GetPublicUrl(string rel) => $"{_basePath}/{rel.Replace('\\', '/')}";
}
