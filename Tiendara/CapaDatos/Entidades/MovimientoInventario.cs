// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Representa la clase movimiento inventario. Con el tiempo ira creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------
using System;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades;

public class MovimientoInventario : IEntidadSql
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InventarioId { get; set; }
    public Guid NegocioId { get; set; }
    public Guid ProductoId { get; set; }
    public Guid? VentaId { get; set; }
    public Guid? CompraId { get; set; }

    public TipoMovimiento Tipo { get; set; }       // Entrada / Salida / Ajuste
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }

    public string? Referencia { get; set; }        // compra#, venta#, motivo ajuste
    public string? Usuario { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
