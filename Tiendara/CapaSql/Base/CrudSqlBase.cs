// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Clase base para todas las entidades SQL que no tienen otra herencia.
// Fecha: 2025-08-19
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System.Data;
using Microsoft.Data.SqlClient;
using Tiendara.CapaContratos;

namespace Tiendara.CapaSql.Base
{
    public abstract class CrudSqlBase<T> : ICrudSql<T> where T : IEntidadSql
    {
        protected readonly string _connStr;
        protected CrudSqlBase(string connectionString) => _connStr = connectionString;

        // Métodos abstractos que cada repo implementa:
        public abstract Task<T?> ObtenerPorIdAsync(Guid id);
        public abstract Task<List<T>> ObtenerTodosAsync();
        public abstract Task InsertarAsync(T entidad);
        public abstract Task ActualizarAsync(T entidad);
        public abstract Task EliminarAsync(Guid id);

        // ===== Helpers reutilizables =====
        protected async Task<SqlConnection> OpenAsync(CancellationToken ct = default)
        {
            var cn = new SqlConnection(_connStr);
            await cn.OpenAsync(ct);
            return cn;
        }

        protected static T2 Get<T2>(IDataRecord r, string col)
        {
            var i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? default! : (T2)r.GetValue(i);
        }

        protected static object DbNullIfNull(object? v) => v ?? DBNull.Value;
    }
}
