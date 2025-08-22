// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Entidad Negocio (antes Tienda): taquería, abarrotes, estética, etc.]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades;

public class Negocio : IEntidadSql
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PropietarioUsuarioId { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string? Giro { get; set; }

    public EstatusMarca EstadoMarca { get; set; } = EstatusMarca.SinRegistrar;
    public string? NombreMarca { get; set; }

    public string? Direccion { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }

    public string? Telefono { get; set; }
    public string? LogoPath { get; set; }  // ej. "C:/Tiendara/Media/Negocios/Logos/{id}.png"
    public string? Notas { get; set; }
    public string? Descripcion { get; set;} 

    public bool Abierto { get; set; } = false;
    public bool Activo { get; set; } = true;

    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public DateTime? ModificadoEn { get; set; }
    public DateTime? UltimoCambioEstado { get; set; }

    public void Abrir()  { Abierto = true;  UltimoCambioEstado = DateTime.UtcNow; ModificadoEn = DateTime.UtcNow; }
    public void Cerrar() { Abierto = false; UltimoCambioEstado = DateTime.UtcNow; ModificadoEn = DateTime.UtcNow; }

    public bool TieneGeoValida()
        => Latitud is >= -90 and <= 90 && Longitud is >= -180 and <= 180;

    public override string ToString() => $"{Nombre} {(Abierto ? "[Abierto]" : "[Cerrado]")}";
}
