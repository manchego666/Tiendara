namespace TiendaraMediaServer.Contracts;

public interface IFotoRepo
{
    Task<string?> GetPerfilPathAsync(Guid usuarioId);
    Task SetPerfilPathAsync(Guid usuarioId, string relativePath);
    Task<string?> GetNegocioPathAsync(Guid negocioId);
    Task SetNegocioPathAsync(Guid negocioId, string relativePath);
}
