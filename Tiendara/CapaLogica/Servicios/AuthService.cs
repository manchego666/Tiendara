// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaLogica.Servicios
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepo _usuarios;

        public AuthService(IUsuarioRepo usuarios) => _usuarios = usuarios;

        public async Task<Usuario> RegistrarAsync(string nombre, string apellidos, string email, string passwordPlano)
        {
            var existente = await _usuarios.GetByEmailAsync(email);
            if (existente is not null) throw new InvalidOperationException("Ya existe un usuario con ese correo.");

            var u = new Usuario { Nombre = nombre, Apellidos = apellidos, Email = email };
            u.EstablecerPassword(passwordPlano); // PBKDF2 de PersonaBase
            await _usuarios.AddAsync(u);
            return u;
        }

        public async Task<Usuario?> LoginAsync(string email, string passwordPlano)
        {
            var u = await _usuarios.GetByEmailAsync(email);
            if (u is null || !u.Activo) return null;
            return u.VerificarPassword(passwordPlano) ? u : null;
        }
    }
}
