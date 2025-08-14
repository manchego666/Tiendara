// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Representa una publicación del feed (usuario o tienda).]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;

namespace Tiendara.CapaDatos.Entidades
{
    public class Publicacion
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Contexto autor / tienda
        public bool EsTienda { get; set; }                      // true = publica una tienda
        public Guid AutorId { get; set; }                       // usuario que publica (si EsTienda=false, o dueño)
        public string? AutorNombre { get; set; }
        public Guid? TiendaId { get; set; }                     // tienda que publica (si EsTienda=true)
        public string? TiendaNombre { get; set; }

        // Contenido
        public string? Texto { get; set; }
        public string? ImagenPath { get; set; }                 // <-- lo que usa tu PublicacionesView

        // Estado + tiempos
        public string Estado { get; set; } = "Publicado";
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}