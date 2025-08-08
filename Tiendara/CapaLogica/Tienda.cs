// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [qué hace esta clase]
/// <summary>
/// Representa a una tienda dentro del sistema.
/// </summary>
// Fecha: [2025-08-08]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;

namespace Tiendara.CapaLogica;

public enum CategoriaTienda
{
    Desconocida = 0,
    Abarrotes = 1,
    Tacos = 2,
    Comida = 3,
    Ropa = 4,
    Electronica = 5,
    Otra = 99
}

public class Tienda
{
    // Identidad
    public Guid TiendaId { get; set; } = Guid.NewGuid();   // GUID de la tienda
    public string Nombre { get; set; } = string.Empty;

    // Propietario (ajusta el tipo según tu Persona.Id)
    // Si  Persona.Id es Guid, cambiar a Guid DuenoId
    public string DuenoId { get; set; } = string.Empty;

    // Clasificación
    public CategoriaTienda Categoria { get; set; } = CategoriaTienda.Desconocida;

    // Ubicación
    public double? Latitud { get; set; }         // null si aún no la fijan
    public double? Longitud { get; set; }        // null si aún no la fijan
    public string? Direccion { get; set; }       // opcional (texto libre)
    public string? Ciudad { get; set; }          // opcional

    // Estado operativo
    public bool EstaAbierta { get; set; } = false;

    // Medios visuales
    public string? LogoLocalPath { get; set; }   // ruta a archivo local (luego  migrar a URL en servidor)

    // --- Métodos de ayuda opcionales ---

    /// <summary>
    /// Valida que Latitud/Longitud tengan un rango válido.
    /// </summary>
    public bool TieneGeoValida()
        => Latitud is >= -90 and <= 90 && Longitud is >= -180 and <= 180;

    /// <summary>
    /// Actualiza coordenadas (devuelve false si no son válidas).
    /// </summary>
    public bool TrySetUbicacion(double lat, double lng)
    {
        if (lat < -90 || lat > 90 || lng < -180 || lng > 180) return false;
        Latitud = lat;
        Longitud = lng;
        return true;
    }
}
