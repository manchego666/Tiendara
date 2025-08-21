// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Helper ADO.NET unificado a Microsoft.Data.SqlClient
// Fecha: 2025-08-19
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Tiendara.CapaSql.Helpers
{
    public static class SqlHelper
    {
        public static SqlConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);

        public static async Task<int> ExecuteAsync(SqlConnection conn, string sql, object? parameters = null, SqlTransaction? tx = null)
        {
            using var cmd = BuildCommand(conn, sql, parameters, tx);
            return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        public static async Task<T?> ExecuteScalarAsync<T>(SqlConnection conn, string sql, object? parameters = null, SqlTransaction? tx = null)
        {
            using var cmd = BuildCommand(conn, sql, parameters, tx);
            var obj = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
            if (obj is null || obj is DBNull) return default;
            return (T)Convert.ChangeType(obj, typeof(T))!;
        }

        public static async Task<List<T>> QueryAsync<T>(SqlConnection conn, string sql, Func<IDataReader, T> map, object? parameters = null, SqlTransaction? tx = null)
        {
            using var cmd = BuildCommand(conn, sql, parameters, tx);
            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            var list = new List<T>();
            while (await reader.ReadAsync().ConfigureAwait(false))
                list.Add(map(reader));
            return list;
        }

        public static async Task<T?> QuerySingleAsync<T>(SqlConnection conn, string sql, Func<IDataReader, T> map, object? parameters = null, SqlTransaction? tx = null)
        {
            using var cmd = BuildCommand(conn, sql, parameters, tx);
            using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
            if (await reader.ReadAsync().ConfigureAwait(false))
                return map(reader);
            return default;
        }

        private static SqlCommand BuildCommand(SqlConnection conn, string sql, object? parameters, SqlTransaction? tx)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            cmd.Transaction = tx;

            if (parameters is not null)
            {
                foreach (var p in parameters.GetType().GetProperties())
                {
                    var name = p.Name.StartsWith("@", StringComparison.Ordinal) ? p.Name : "@" + p.Name;
                    var value = p.GetValue(parameters) ?? DBNull.Value;
                    cmd.Parameters.AddWithValue(name, value);
                }
            }
            return cmd;
        }
    }
}
