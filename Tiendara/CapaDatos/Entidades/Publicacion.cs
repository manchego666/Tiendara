// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Representa una publicación del feed (usuario o tienda).
// Fecha: 2025-08-10
// ------------------------------------------------------------
using System;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades
{
    public class Publicacion : IEntidadSql
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Tipo + región (lo usa el feed/marketing)
        public PublicationType Type { get; set; } = PublicationType.Noticias;
        public Ubicacion Location { get; set; } = new();

        // Contexto autor / tienda
        public bool EsTienda { get; set; }
        public Guid AutorId { get; set; }
        public string? AutorNombre { get; set; }
        public Guid? TiendaId { get; set; }
        public string? TiendaNombre { get; set; }

        // Contenido
        public string? Texto { get; set; }
        public string? ImagenPath { get; set; } // usa URL si guardas en blob/filesystem

        // Estado + tiempos
        public string Estado { get; set; } = "Publicado";
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? ModificadoEn { get; set; }
    }
}
