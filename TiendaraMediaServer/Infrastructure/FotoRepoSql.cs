using Microsoft.Data.SqlClient;
using TiendaraMediaServer.Contracts;

namespace TiendaraMediaServer.Infrastructure;

public sealed class FotoRepoSql : IFotoRepo
{
    private readonly string _conn;
    public FotoRepoSql(string conn) => _conn = conn;

    // ====== USUARIO ======
    public async Task<string?> GetPerfilPathAsync(Guid usuarioId)
    {
        using var cn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("SELECT AvatarPath FROM Usuario WHERE Id=@Id", cn);
        cmd.Parameters.AddWithValue("@Id", usuarioId);
        await cn.OpenAsync();
        return await cmd.ExecuteScalarAsync() as string;
    }

    public async Task SetPerfilPathAsync(Guid usuarioId, string relativePath)
    {
        using var cn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("UPDATE Usuario SET AvatarPath=@P WHERE Id=@Id", cn);
        cmd.Parameters.AddWithValue("@P", relativePath);
        cmd.Parameters.AddWithValue("@Id", usuarioId);
        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }


    // ====== NEGOCIO ======
    public async Task<string?> GetNegocioPathAsync(Guid negocioId)
    {
        using var cn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("SELECT LogoPath FROM Negocio WHERE Id=@Id", cn);
        cmd.Parameters.AddWithValue("@Id", negocioId);
        await cn.OpenAsync();
        return await cmd.ExecuteScalarAsync() as string;
    }

    public async Task SetNegocioPathAsync(Guid negocioId, string relativePath)
    {
        using var cn = new SqlConnection(_conn);
        using var cmd = new SqlCommand("UPDATE Negocio SET LogoPath=@P WHERE Id=@Id", cn);
        cmd.Parameters.AddWithValue("@P", relativePath);
        cmd.Parameters.AddWithValue("@Id", negocioId);
        await cn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

}
