// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Clase de venta y esta clase no tira piedritas ni manda mensajes tiene total relatividad a su clase -.-' .[uwu]. Con el tiempo ira creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades;

public class Venta : IEntidadSql
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NegocioId { get; set; }
    public Guid VendedorUsuarioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public List<VentaLinea> Lineas { get; set; } = new();

    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }

    public MedioPago MedioPago { get; set; } = MedioPago.Efectivo;
    public decimal? PagoRecibido { get; set; }
    public decimal? Cambio { get; set; }

    public string? Referencia { get; set; }
    public EstadoVenta Estado { get; set; } = EstadoVenta.Finalizada;

    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public DateTime? ModificadoEn { get; set; }
}

public class VentaLinea
{
    public Guid ItemId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public UnidadVenta Unidad { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal TasaImpuesto { get; set; }
    public decimal Importe { get; set; } // PrecioUnitario * Cantidad
}
