// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Representa una clase para ayuda en movimiento de cajas y auditar. Con el tiempo irá creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades;

public class MovimientoCaja : IEntidadSql
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NegocioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public TipoMovimientoCaja Tipo { get; set; }
    public decimal Monto { get; set; }
    public MedioPago Medio { get; set; } = MedioPago.Efectivo;

    public string? Concepto { get; set; }
    public Guid? VentaId { get; set; }
    public string? Usuario { get; set; }
}
