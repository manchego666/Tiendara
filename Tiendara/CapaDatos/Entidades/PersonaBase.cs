// --------------------------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Clase base para personas (plantilla). Maneja hash de contraseña.
// Fecha: 2025-08-10
// Derechos reservados © ZORRODEV - 2025
// ---------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

namespace Tiendara.CapaDatos.Entidades
{
    public abstract class PersonaBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;

        public string? Rfc { get; set; }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => _email = (value ?? string.Empty).Trim().ToLowerInvariant();
        }

        public string? Telefono { get; set; }

        public string? Avatar { get; set; }
        public string? Foto { get; set; }

        public string? HuellaHashBase64 { get; set; }

        // Credenciales
        public string? PasswordSaltBase64 { get; set; }
        public string? PasswordHashBase64 { get; set; }
        public int PasswordIterations { get; set; } = 100_000;

        public bool Activo { get; set; } = true;
        public DateTime CreadoEn { get; set; } = DateTime.Now;
        public DateTime? ModificadoEn { get; set; }

        // ---------- Utilidades de perfil ----------
        public void ActualizarContacto(string? telefono, string? email)
        {
            Telefono = telefono?.Trim();
            Email = email ?? string.Empty;
            ModificadoEn = DateTime.Now;
        }

        public void ActualizarIdentificadores(string? rfc = null, string? avatar = null, string? foto = null)
        {
            Rfc = rfc?.Trim();
            Avatar = avatar?.Trim();
            Foto = foto?.Trim();
            ModificadoEn = DateTime.Now;
        }

        public void DefinirHuellaHash(string? huellaHashBase64)
        {
            HuellaHashBase64 = string.IsNullOrWhiteSpace(huellaHashBase64) ? null : huellaHashBase64.Trim();
            ModificadoEn = DateTime.Now;
        }

        // ---------- Password ----------
        public void EstablecerPassword(string passwordPlano)
        {
            if (string.IsNullOrWhiteSpace(passwordPlano))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(passwordPlano));

            // 128-bit salt
            var salt = RandomNumberGenerator.GetBytes(16);

            // Asegura mínimo 100k iteraciones y guarda el valor
            PasswordIterations = Math.Max(PasswordIterations, 100_000);

            // 256-bit hash con SHA256 y UTF-8
            var hash = PBKDF2(passwordPlano, salt, PasswordIterations, 32);

            PasswordSaltBase64 = Convert.ToBase64String(salt);
            PasswordHashBase64 = Convert.ToBase64String(hash);
            ModificadoEn = DateTime.Now;
        }

        public bool VerificarPassword(string passwordPlano)
        {
            if (string.IsNullOrWhiteSpace(PasswordSaltBase64) ||
                string.IsNullOrWhiteSpace(PasswordHashBase64) ||
                PasswordIterations <= 0)
                return false;

            var salt = Convert.FromBase64String(PasswordSaltBase64);
            var esperado = Convert.FromBase64String(PasswordHashBase64);

            var actual = PBKDF2(passwordPlano, salt, PasswordIterations, esperado.Length);

            return CryptographicOperations.FixedTimeEquals(actual, esperado);
        }

        // Helper PBKDF2 (SHA256, UTF-8)
        private static byte[] PBKDF2(string password, byte[] salt, int iter, int len)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iter,
                HashAlgorithmName.SHA256,
                len
            );
        }
    }
}
