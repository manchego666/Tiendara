using System;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades
{
    public class ChatThread : IEntidadSql
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AId { get; set; }
        public bool AEsTienda { get; set; }

        public Guid BId { get; set; }
        public bool BEsTienda { get; set; }

        public DateTime UltimoMensajeEn { get; set; } = DateTime.UtcNow;
    }
}
