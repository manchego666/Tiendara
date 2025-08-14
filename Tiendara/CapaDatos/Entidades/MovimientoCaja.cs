using System;

namespace Tiendara.CapaDatos.Entidades;

public class MovimientoCaja
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NegocioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;

    public TipoMovimientoCaja Tipo { get; set; }
    public decimal Monto { get; set; }
    public MedioPago Medio { get; set; } = MedioPago.Efectivo;

    public string? Concepto { get; set; }
    public Guid? VentaId { get; set; }
    public string? Usuario { get; set; }
}
