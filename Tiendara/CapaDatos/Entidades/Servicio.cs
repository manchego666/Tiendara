// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Representa una clase para servicios como corte de pelo, plomeria , electricidad, limpieza. Con el tiempo irá creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades;

public class Servicio : IVendible,IEntidadSql
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nombre { get; set; } = string.Empty;
    public CategoriaServicio Categoria { get; set; } = CategoriaServicio.Otros;
    public string? Descripcion { get; set; }
    public int? DuracionMinutos { get; set; }

    public UnidadVenta UnidadVenta { get; set; } = UnidadVenta.Servicio;
    public decimal PrecioBase { get; set; }
    public decimal TasaImpuesto { get; set; } = 0m;
    public bool EsPesable { get; } = false;

    public override string ToString() => $"{Nombre} (servicio)";
}
