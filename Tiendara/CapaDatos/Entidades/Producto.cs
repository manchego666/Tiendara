// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Manejamos la clase producto sus atributos y primeras propiedades.[uwu]. Con el tiempo ira creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------


using System;
using System.Collections.Generic;

namespace Tiendara.CapaDatos.Entidades;

public class Producto : IVendible
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Identificación
    public string SKU { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string CategoriaTexto { get; set; } = string.Empty;
    public CategoriaProducto Categoria { get; set; } = CategoriaProducto.Otros;

    // Venta (IVendible)
    public UnidadVenta UnidadVenta { get; set; } = UnidadVenta.Pza;
    public decimal PrecioBase { get; set; }
    public decimal TasaImpuesto { get; set; } = 0.16m;
    public bool EsPesable { get; set; } = false;

    // Extras
    public bool EsPerecedero { get; set; } = false;
    public List<Guid> ComponentesProductoIds { get; set; } = new();

    public override string ToString() => $"{Nombre} ({SKU})";
}
