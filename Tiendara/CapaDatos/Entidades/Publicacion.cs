// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Representa una publicación del feed (usuario o tienda).
// Fecha: 2025-08-10
// ------------------------------------------------------------
using System;

namespace Tiendara.CapaDatos.Entidades
{
    public class Publicacion
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Tipo + región (lo usa el ContenedorMarketingView)
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
        public string? ImagenPath { get; set; } // usado por el feed

        // Estado + tiempos
        public string Estado { get; set; } = "Publicado";
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
