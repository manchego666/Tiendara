using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaVisual.Utils
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
