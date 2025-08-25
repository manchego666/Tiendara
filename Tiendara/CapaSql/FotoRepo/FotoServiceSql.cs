using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiendara.CapaContratos;

namespace Tiendara.CapaSql.FotoRepo
{
    public sealed class FotoServiceSql : IFotoRepo
    {
        private readonly string _conn;
        public FotoServiceSql(string conn) => _conn = conn;

        public async Task<string?> GetPerfilPathAsync(Guid usuarioId)
        {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT PerfilFotoPath FROM Usuario WHERE Id=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", usuarioId);
            await cn.OpenAsync();
            return await cmd.ExecuteScalarAsync() as string;
        }

        public async Task SetPerfilPathAsync(Guid usuarioId, string relativePath)
        {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("UPDATE Usuario SET PerfilFotoPath=@P WHERE Id=@Id", cn);
            cmd.Parameters.AddWithValue("@P", relativePath);
            cmd.Parameters.AddWithValue("@Id", usuarioId);
            await cn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<string?> GetNegocioPathAsync(Guid negocioId)
        {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SELECT PortadaPath FROM Negocio WHERE Id=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", negocioId);
            await cn.OpenAsync();
            return await cmd.ExecuteScalarAsync() as string;
        }

        public async Task SetNegocioPathAsync(Guid negocioId, string relativePath)
        {
            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("UPDATE Negocio SET PortadaPath=@P WHERE Id=@Id", cn);
            cmd.Parameters.AddWithValue("@P", relativePath);
            cmd.Parameters.AddWithValue("@Id", negocioId);
            await cn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}