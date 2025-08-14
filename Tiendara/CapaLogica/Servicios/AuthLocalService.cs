// ------------------------------------------------------------
// Proyecto: Tiendara
// Archivo: CapaLogica/Servicios/AuthLocalService.cs
// ------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaDatos.Repos;

namespace Tiendara.CapaLogica.Servicios
{
    public sealed class AuthLocalService
    {
        private readonly IUsuarioRepo _repo;
        public AuthLocalService(IUsuarioRepo repo) => _repo = repo;

        public async Task<(bool ok, string? error, Usuario? user)> RegistrarAsync(string nombre, string email, string password)
        {
            nombre = (nombre ?? "").Trim();
            email = (email ?? "").Trim().ToLowerInvariant();
            password = password ?? "";

            if (string.IsNullOrWhiteSpace(nombre)) return (false, "Escribe tu nombre.", null);
            if (string.IsNullOrWhiteSpace(email)) return (false, "Escribe tu correo.", null);
            if (password.Length < 6) return (false, "La contraseña debe tener al menos 6 caracteres.", null);

            var existente = await _repo.GetByEmailAsync(email);
            if (existente != null) return (false, "Ya existe una cuenta con ese correo.", null);

            var u = new Usuario
            {
                Nombre = nombre,
                Email = email
                // Roles inicia con Tiendaro por defecto en tu clase
            };

            // Hashea y guarda salt/hash en PersonaBase
            u.EstablecerPassword(password);

            await _repo.AddAsync(u);
            SesionActual.Establecer(u);
            return (true, null, u);
        }

        public async Task<(bool ok, string? error, Usuario? user)> LoginAsync(string email, string password)
        {
            email = (email ?? "").Trim().ToLowerInvariant();
            password = password ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Correo y contraseña son requeridos.", null);

            var u = await _repo.GetByEmailAsync(email);
            if (u == null) return (false, "No existe una cuenta con ese correo.", null);

            if (!u.VerificarPassword(password))
                return (false, "Contraseña incorrecta.", null);


            SesionActual.Establecer(u);
            return (true, null, u);
        }


        public void Logout() => SesionActual.Cerrar();
    }

    public static class SesionActual
    {
        public static Usuario? Usuario { get; private set; }
        public static bool Autenticado => Usuario != null;
        public static Guid UsuarioId => Usuario?.Id ?? Guid.Empty;

        public static void Establecer(Usuario u) => Usuario = u;
        public static void Cerrar() => Usuario = null;
    }
}
