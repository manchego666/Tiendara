using System;
using System.Threading;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaLogica.Servicios
{
    // ===== Inventario =====
    public interface IInventarioService
    {
        Task RegistrarEntrada(Guid negocioId, Guid productoId, decimal cantidad, decimal costoUnitario,
                              string? referencia = null, string? usuario = null);

        Task RegistrarSalida(Guid negocioId, Guid productoId, decimal cantidad,
                             Guid? ventaId = null, string? referencia = null, string? usuario = null);

        Task Ajustar(Guid negocioId, Guid productoId, decimal ajusteCantidad, string motivo, string? usuario = null);
    }

    // ===== Caja =====
    public interface ICajaService
    {
        Task<Guid> RegistrarRetiro(Guid negocioId, decimal monto, string concepto,
                                   string? usuario = null, MedioPago medio = MedioPago.Efectivo);

        Task<CorteCaja> GenerarCorte(Guid negocioId, DateTime desde, DateTime hasta);
    }

    // ===== Ventas =====
    // Usa IVendedor y SolicitudVenta definidos en CapaDatos.Entidades (Interfaces.cs/Usuario.cs)
    public interface IVentaService
    {
        Task<Guid> RegistrarVentaAsync(IVendedor vendedor, SolicitudVenta s, CancellationToken ct = default);
    }
}
