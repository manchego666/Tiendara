namespace TiendaraMediaServer.Contracts;

public enum MediaArea { UsuarioAvatar, NegocioLogo }

public interface IFotoStorage
{
    Task<string> SaveAsync(MediaArea area, Guid id, Stream file, string ext);
    Task DeleteIfExistsAsync(string relativePath);
    bool Exists(string relativePath);
    string GetPublicUrl(string relativePath);
}
