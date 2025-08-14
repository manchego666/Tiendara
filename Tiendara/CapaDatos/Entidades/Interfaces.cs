// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Manejamos las interfaces en esta clase. Con el tiempo ira creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tiendara.CapaDatos.Entidades;

// ===== Roles / capacidades de negocio =====
public interface IVendedor
{
    Guid UsuarioId { get; }
    bool PuedeVender { get; }

    Task<Guid> VenderAsync(SolicitudVenta solicitud, CancellationToken ct = default);
}

public sealed class SolicitudVenta
{
    public Guid? NegocioId { get; set; }          // a qué negocio aplica
    public string? Referencia { get; set; }       // opcional (ticket, nota)
    public MedioPago MedioPago { get; set; } = MedioPago.Efectivo;
    public decimal PagoRecibido { get; set; }     // para calcular cambio (Total lo calcula el servicio)
    public List<LineaVentaInput> Lineas { get; set; } = new();
}

public sealed class LineaVentaInput
{
    public Guid ItemId { get; set; }              // Id del IVendible (Producto/Servicio)
    public string? Descripcion { get; set; }      // texto libre opcional (servicio custom)
    public decimal Cantidad { get; set; }         // piezas, kg, lt, etc.
    public decimal PrecioUnitario { get; set; }   // si 0, el servicio tomará el del catálogo
    public decimal? TasaImpuesto { get; set; }    // si null, usar la del item
}

public interface IVendible
{
    Guid Id { get; }
    string Nombre { get; }
    UnidadVenta UnidadVenta { get; }
    decimal PrecioBase { get; }
    decimal TasaImpuesto { get; }
    bool EsPesable { get; }
}
