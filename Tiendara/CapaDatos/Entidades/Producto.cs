// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Representa una clase con sus interces y atributos importantes. Con el tiempo irá creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades;

public class Producto : IVendible, IEntidadSql
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Identificación
    public string SKU { get; set; } = string.Empty;
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string CategoriaTexto { get; set; } = string.Empty;
    public CategoriaProducto Categoria { get; set; } = CategoriaProducto.Otros;

    // Venta
    public UnidadVenta UnidadVenta { get; set; } = UnidadVenta.Pza;
    public TipoMedida TipoMedida { get; set; } = TipoMedida.Unidad;
    public decimal PrecioBase { get; set; }
    public decimal TasaImpuesto { get; set; } = 0.16m;

    // Estado y disponibilidad
    public EstadoProducto Estado { get; set; } = EstadoProducto.Activo;
    public bool EsPerecedero { get; set; } = false;
    public DateTime? FechaVencimiento { get; set; }

    public bool EsPesable => TipoMedida.HasFlag(TipoMedida.Peso);
    public bool EsMedible => TipoMedida != TipoMedida.Unidad && TipoMedida != TipoMedida.Ninguna;

    // Oferta
    public TipoOferta Oferta { get; set; } = TipoOferta.Ninguna;
    public decimal? ValorOferta { get; set; }
    public DateTime? FechaInicioOferta { get; set; }
    public DateTime? FechaFinOferta { get; set; }

    // Composición
    public List<Guid> ComponentesProductoIds { get; set; } = new();

    // Métodos auxiliares
    public bool TieneOfertaActiva()
    {
        if (Oferta == TipoOferta.Ninguna || !FechaInicioOferta.HasValue || !FechaFinOferta.HasValue)
            return false;

        var ahora = DateTime.UtcNow;
        return ahora >= FechaInicioOferta && ahora <= FechaFinOferta;
    }

    public decimal ObtenerPrecioFinal()
    {
        if (TieneOfertaActiva() && ValorOferta.HasValue)
        {
            return Oferta == TipoOferta.Descuento
                ? PrecioBase * (1 - ValorOferta.Value)
                : PrecioBase;
        }
        return PrecioBase;
    }

    public bool EstaDisponibleParaVenta()
    {
        if (Estado != EstadoProducto.Activo || PrecioBase <= 0)
            return false;

        if (EsPerecedero && FechaVencimiento.HasValue && FechaVencimiento < DateTime.UtcNow)
            return false;

        return true;
    }

    public override string ToString() => $"{Nombre} ({SKU})";

    public string DescripcionCompleta()
        => $"{Nombre} - {Marca} ({SKU}) | {UnidadVenta} | Precio: {ObtenerPrecioFinal():C}";
}
