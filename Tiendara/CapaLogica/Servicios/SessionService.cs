// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaLogica.Servicios
{
    public class SessionService
    {
        // Usuario logeado en memoria
        public Usuario? UsuarioLogeado { get; private set; }

        // Guardar sesión
        public void SetUsuario(Usuario u) => UsuarioLogeado = u;

        // Cerrar sesión
        public void Logout() => UsuarioLogeado = null;
    }
}
