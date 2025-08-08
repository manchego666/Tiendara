// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Modelo base para representar una persona
//              (dueño, proveedor, empleado).
// Fecha: 2025-08-05
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Tiendara.CapaLogica
{
    /// <summary>
    /// Clase base para representar una persona del sistema.
    /// Puede ser usada para empleados, dueños, proveedores, etc.
    /// </summary>
    public abstract class Persona
    {
        // Identidad y tipo
        public string Id { get; set; } = string.Empty;                 // También puede usarse para login
        public TipoPersona Tipo { get; set; } = TipoPersona.Dueno;     // No editable por el usuario

        // Datos principales
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;

        // Contacto (opcionales por ahora)
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }

        // Perfil
        public DateOnly? FechaNacimiento { get; set; }                 // Opcional
        public bool BiometriaActivada { get; set; } = false;           // Preferencia local
        public string? FotoLocalPath { get; set; }                     // Ruta local en el dispositivo

        // Relaciones con tiendas
        /// <summary>
        /// Tiendas de las que es dueña la persona (aplica a Dueño). Vacío si no tiene.
        /// </summary>
        public List<Guid> TiendasPropiasIds { get; init; } = new();

        /// <summary>
        /// Tiendas a las que está asignada la persona (Empleado/Proveedor).
        /// </summary>
        public List<Guid> TiendasAsignadasIds { get; init; } = new();

        // Helpers de UI
        public string NombreCompleto =>
            string.IsNullOrWhiteSpace(Apellidos) ? Nombre : $"{Nombre} {Apellidos}";

        public int? Edad => FechaNacimiento.HasValue ? CalcularEdad(FechaNacimiento.Value) : null;

        private static int CalcularEdad(DateOnly fecha)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var edad = hoy.Year - fecha.Year;
            if (fecha > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        public override string ToString() => $"{NombreCompleto} ({Tipo})";

        // Métodos de conveniencia (opcionales)
        public bool AgregarTiendaPropia(Guid tiendaId)
        {
            if (tiendaId == Guid.Empty) return false;
            if (!TiendasPropiasIds.Contains(tiendaId))
                TiendasPropiasIds.Add(tiendaId);
            return true;
        }

        public bool AgregarTiendaAsignada(Guid tiendaId)
        {
            if (tiendaId == Guid.Empty) return false;
            if (!TiendasAsignadasIds.Contains(tiendaId))
                TiendasAsignadasIds.Add(tiendaId);
            return true;
        }
    }

    public enum TipoPersona
    {
        Desconocido = 0,
        Dueno = 1,
        Proveedor = 2,
        Empleado = 3
    }
}
