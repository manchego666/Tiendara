// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Representa la clase para Mensaje. Con el tiempo irá creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades
{
    public class Mensaje : IEntidadSql
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ThreadId { get; set; }
        public Guid AutorId { get; set; }
        public bool AutorEsTienda { get; set; }

        public string Texto { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }

        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
