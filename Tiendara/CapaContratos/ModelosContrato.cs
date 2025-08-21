using System;
using System.Collections.Generic;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaContratos;

    public class SolicitudVenta
    {
        public Guid NegocioId { get; set; }

        // NUEVO: forma de pago que eligió la UI
        public MedioPago MedioPago { get; set; } = MedioPago.Efectivo;

        // Si es efectivo usamos esto; si es tarjeta, úsalo como "monto a cobrar"
        public decimal PagoRecibido { get; set; }

        // NUEVO: para tarjetas/transferencias (referencia del proveedor)
        public string? PagoReferencia { get; set; }  // p.ej. auth code del POS/banco

        public List<LineaVentaInput> Lineas { get; set; } = new();
    }

    public class LineaVentaInput
    {
        public Guid ItemId { get; set; }
        public decimal Cantidad { get; set; }
    }

    public class CorteCaja
    {
        public Guid NegocioId { get; set; }
        public DateTime Desde { get; set; }   // UTC
        public DateTime Hasta { get; set; }   // UTC
        public decimal SaldoInicial { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalRetiros { get; set; }
        public decimal SaldoFinal { get; set; }
        public int Movimientos { get; set; }
    }
