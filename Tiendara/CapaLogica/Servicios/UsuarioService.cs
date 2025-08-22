using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaLogica.Servicios
{
    public class UsuarioService
    {
        private readonly IUsuarioRepo _usuarios;

        public UsuarioService(IUsuarioRepo usuarios) => _usuarios = usuarios;

        public async Task<Usuario?> GetByIdAsync(Guid id) => await _usuarios.GetByIdAsync(id);



        public async Task ActualizarDatosAsync(Guid id, string nombre, string apellidos)
        {
            var u = await _usuarios.GetByIdAsync(id);
            if (u is null) throw new InvalidOperationException("Usuario no encontrado");
            u.Nombre = nombre;
            u.Apellidos = apellidos;
            await _usuarios.UpdateAsync(u);
        }

        public async Task CambiarPasswordAsync(Guid id, string passwordViejo, string passwordNuevo)
        {
            var u = await _usuarios.GetByIdAsync(id);
            if (u is null || !u.VerificarPassword(passwordViejo))
                throw new UnauthorizedAccessException("Contraseña incorrecta");
            u.EstablecerPassword(passwordNuevo);
            await _usuarios.UpdateAsync(u);
        }

    }
}
