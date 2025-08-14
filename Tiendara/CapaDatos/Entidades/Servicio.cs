using System;

namespace Tiendara.CapaDatos.Entidades;

public class Servicio : IVendible
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
