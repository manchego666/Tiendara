// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaLogica.Servicios
{
    namespace Tiendara.CapaLogica.Servicios
    {
        public class SessionService
        {
            public Usuario? UsuarioActual { get; private set; }
            public bool Autenticado => UsuarioActual is not null;
            public Guid UsuarioId => UsuarioActual?.Id ?? Guid.Empty;

            public void Set(Usuario u) => UsuarioActual = u;
            public void Clear() => UsuarioActual = null;
        }
    }
}