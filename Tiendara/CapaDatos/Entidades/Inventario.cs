// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Representa la clase Inventario y sus atributos. Con el tiempo irá creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------
using System;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades;

public class Inventario : IEntidadSql
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Relaciones
    public Guid NegocioId { get; set; }
    public Guid ProductoId { get; set; }
    public Guid? LoteId { get; set; }

    // Niveles
    public decimal CantidadDisponible { get; set; }
    public decimal CantidadReservada { get; set; }
    public decimal Minimo { get; set; }
    public decimal Maximo { get; set; }
    public string? Ubicacion { get; set; }

    // Costos
    public decimal CostoPromedio { get; set; }
    public decimal CostoUltimaCompra { get; set; }

    // Auditoría
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public DateTime? ModificadoEn { get; set; }

    public decimal DisponibleParaVenta => Math.Round(CantidadDisponible - CantidadReservada, 2);

    public override string ToString()
        => $"Inv[{ProductoId}] Neg:{NegocioId} Disp:{CantidadDisponible} Cp:{CostoPromedio}";
}
