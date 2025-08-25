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
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaSql.Base;
using Tiendara.CapaSql.Conexion;

namespace Tiendara.CapaSql.UsuarioRepo
{
    public class UsuarioServiceSql : CrudSqlBase<Usuario>, IUsuarioRepo
    {
        public UsuarioServiceSql() : base(ConfiguracionSql.ConnectionString) { }

        // ===== CRUD base =====

        public override async Task<Usuario?> ObtenerPorIdAsync(Guid id)
        {
            const string sql = @"SELECT TOP(1) * FROM dbo.Usuario WHERE Id=@Id;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var rd = await cmd.ExecuteReaderAsync();
            return await rd.ReadAsync() ? Map(rd) : null;
        }

        public override async Task<List<Usuario>> ObtenerTodosAsync()
        {
            const string sql = @"SELECT * FROM dbo.Usuario ORDER BY CreadoEn DESC;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<Usuario>();
            while (await rd.ReadAsync())
                list.Add(Map(rd));
            return list;
        }

        public override async Task InsertarAsync(Usuario e)
        {
            if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();

            const string sql = @"
INSERT INTO dbo.Usuario
(Id, Nombre, Apellidos, Rfc, Email, Telefono, AvatarPath, HuellaHashBase64,
 PasswordSaltBase64, PasswordHashBase64, PasswordIterations, Activo, CreadoEn)
VALUES
(@Id, @Nombre, @Apellidos, @Rfc, @Email, @Telefono, @AvatarPath, @HuellaHashBase64,
 @PasswordSaltBase64, @PasswordHashBase64, @PasswordIterations, @Activo, SYSUTCDATETIME());";

            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            AddCommonParams(cmd, e, includeId: true);
            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task ActualizarAsync(Usuario e)
        {
            const string sql = @"
UPDATE dbo.Usuario SET
  Nombre=@Nombre,
  Apellidos=@Apellidos,
  Rfc=@Rfc,
  Email=@Email,
  Telefono=@Telefono,
  AvatarPath=@AvatarPath,
  HuellaHashBase64=@HuellaHashBase64,
  PasswordSaltBase64=@PasswordSaltBase64,
  PasswordHashBase64=@PasswordHashBase64,
  PasswordIterations=@PasswordIterations,
  Activo=@Activo,
  ModificadoEn=SYSUTCDATETIME()
WHERE Id=@Id;";

            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            AddCommonParams(cmd, e, includeId: true);
            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task EliminarAsync(Guid id)
        {
            // Borrado lógico
            const string sql = @"UPDATE dbo.Usuario SET Activo=0, ModificadoEn=SYSUTCDATETIME() WHERE Id=@Id;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        // ===== API específica (IUsuarioRepo) =====

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            const string sql = @"SELECT TOP(1) * FROM dbo.Usuario WHERE Email=@Email;";
            using var cn = await OpenAsync();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Email", (email ?? string.Empty).Trim().ToLowerInvariant());
            using var rd = await cmd.ExecuteReaderAsync();
            return await rd.ReadAsync() ? Map(rd) : null;
        }

        public async Task<Usuario?> GetByIdAsync(Guid id) => await ObtenerPorIdAsync(id);
        public async Task AddAsync(Usuario u) => await InsertarAsync(u);
        public async Task UpdateAsync(Usuario u) => await ActualizarAsync(u);
        public async Task<List<Usuario>> GetAllAsync() => await ObtenerTodosAsync();

        // ===== Helpers =====

        private static Usuario Map(IDataRecord r) => new Usuario
        {
            Id = Get<Guid>(r, nameof(Usuario.Id)),
            Nombre = Get<string>(r, nameof(Usuario.Nombre)) ?? string.Empty,
            Apellidos = Get<string>(r, nameof(Usuario.Apellidos)) ?? string.Empty,
            Rfc = Get<string>(r, nameof(Usuario.Rfc)),
            Email = Get<string>(r, nameof(Usuario.Email)) ?? string.Empty,
            Telefono = Get<string>(r, nameof(Usuario.Telefono)),
            AvatarPath = Get<string>(r, nameof(Usuario.AvatarPath)),
            HuellaHashBase64 = Get<string>(r, nameof(Usuario.HuellaHashBase64)),
            PasswordSaltBase64 = Get<string>(r, nameof(Usuario.PasswordSaltBase64)),
            PasswordHashBase64 = Get<string>(r, nameof(Usuario.PasswordHashBase64)),
            PasswordIterations = Get<int>(r, nameof(Usuario.PasswordIterations)),
            Activo = Get<bool>(r, nameof(Usuario.Activo)),
            CreadoEn = Get<DateTime>(r, nameof(Usuario.CreadoEn)),
            ModificadoEn = Get<DateTime?>(r, nameof(Usuario.ModificadoEn))
        };

        private static void AddCommonParams(SqlCommand cmd, Usuario e, bool includeId)
        {
            if (includeId) cmd.Parameters.AddWithValue("@Id", e.Id);

            cmd.Parameters.AddWithValue("@Nombre", e.Nombre ?? string.Empty);
            cmd.Parameters.AddWithValue("@Apellidos", e.Apellidos ?? string.Empty);
            cmd.Parameters.AddWithValue("@Rfc", (object?)e.Rfc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", e.Email ?? string.Empty);
            cmd.Parameters.AddWithValue("@Telefono", (object?)e.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AvatarPath", (object?)e.AvatarPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HuellaHashBase64", (object?)e.HuellaHashBase64 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PasswordSaltBase64", (object?)e.PasswordSaltBase64 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PasswordHashBase64", (object?)e.PasswordHashBase64 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PasswordIterations", e.PasswordIterations);
            cmd.Parameters.AddWithValue("@Activo", e.Activo);
        }
    }
}
