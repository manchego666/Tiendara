// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Manejamos los enums y roles de usuario en esta clase. Con el tiempo ira creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiendara.CapaDatos.Entidades
{
    /// <summary>
    /// Un usuario puede tener varios roles a la vez.
    /// Inicia como Tiendaro y puede sumar Empleado, Proveedor, Emprendedor, Profesionista y/o Tecnico.
    /// </summary>
    public enum RolUsuario
    {
        UsuarioNormal = 0,    // usuario base que accede a la app
        Empleado = 1,    // personal operativo de una tienda/negocio
        Proveedor = 2,   // vende/surte a tiendas o clientes
        Emprendedor = 3, // dueño/propietario de negocio (tacos, sushi, abarrotes, etc.)
        Profesionista = 4, // profesional con cédula/oficio formal (usa enum Profesion)
        Tecnico = 5        // técnico/oficio práctico (usa enum OficioTecnico)
    }

    /// <summary>
    /// Forma jurídica/operativa del proveedor (UNA sola entidad Proveedor con esta distinción).
    /// - PersonaFisica: p. ej., Plácido como persona que provee.
    /// - Empresa: p. ej., "Producto Regional Sinaloense" (dueño: Plácido), "Coca-Cola", "Sabritas".
    /// </summary>
    public enum FormaProveedor
    {
        PersonaFisica = 0,
        Empresa = 1
    }

    /// <summary>
    /// Estado de la marca del proveedor/empresa.
    /// </summary>
    public enum EstatusMarca
    {
        SinRegistrar = 0,
        Registrada = 1
    }

    /// <summary>
    /// Profesiones para el rol Profesionista.
    /// </summary>
    public enum Profesion
    {
        Ninguna = 0,
        Doctor,
        Dentista,
        Estilista,
        Abogado,
        Contador,
        Arquitecto,
        Ingeniero,
        Nutriologo,
        Psicologo,
        Chef,
        Otros
    }

    /// <summary>
    /// Oficios técnicos para el rol Tecnico.
    /// </summary>
    public enum OficioTecnico
    {
        Ninguno = 0,
        Plomero,
        Carpintero,
        Electricista,
        Soldador,
        Mecanico,
        TecnicoPC,
        TecnicoRedes,
        ReparacionCelulares,
        Herrero,
        Albanil,
        Otros
    }

    /// <summary>
    /// Unidad principal con la que se vende el producto o servicio.
    /// Visible en etiquetas, venta, catálogos, etc.
    /// </summary>
    public enum UnidadVenta
    {
        Pza = 0,      // piezas
        Kg = 1,       // kilogramos
        G = 2,        // gramos
        Lt = 3,       // litros
        Ml = 4,       // mililitros
        M = 5,        // metros (ej: tela)
        Servicio = 6  // aplica para servicios (masaje, corte, etc.)
    }


    /// <summary>
    /// Categorías básicas para productos físicos.
    /// </summary>
    public enum CategoriaProducto
    {
        Frutas,
        Carnes,
        Quesos,
        PanPasteles,
        RaspadosHelados,
        Ropa,
        Electronica,
        CocinaUtensilios,
        Abarrotes,
        Belleza,
        Farmacia,
        Otros
    }

    /// <summary>
    /// Categorías para servicios (clasificación del catálogo de servicios).
    /// </summary>
    public enum CategoriaServicio
    {
        Estilista,
        Masajes,
        ClasesNinosEspeciales,
        ConsultaDental,
        Reparacion,
        Otros
    }

    public enum TipoMovimiento
    {
        Entrada,
        Salida,
        Ajuste
         
    }

    public enum EstadoVenta 
    {
        Abierta = 0,
        Finalizada = 1,
        Cancelada = 2
    }

    public enum MedioPago 
    { 
        Efectivo = 0,
        Tarjeta = 1,
        Transferencia = 2,
        Mixto = 3 
    }

    public enum TipoMovimientoCaja 
    { 
        IngresoVenta = 0,
        Retiro = 1,
        Ajuste = 2
    }

    public enum PublicationType
    {
        Noticias = 0,
        Empleos = 1,
        Promociones = 2,
        VentasRapidas = 3,
        Servicios = 4,
        Ofertas = 5,
        Hot = 6
    }



    public enum NavTab
    {
        None = -1,
        Home = 0,
        World = 1,
        Chat = 2,
        Bell = 3
    }

    /// <summary>
    /// Define las medidas físicas que puede tener un producto o servicio.
    /// Puede tener múltiples banderas (flags) a la vez.
    /// Ej: un garrafón puede tener Peso y Volumen.
    /// </summary>
    [Flags]
    public enum TipoMedida
    {
        Ninguna = 0,
        Unidad = 1 << 0,   // piezas, cantidades sueltas
        Peso = 1 << 1,     // gramos, kilos
        Volumen = 1 << 2,  // litros, mililitros
        Longitud = 1 << 3, // metros, centímetros
        Porcion = 1 << 4   // corte, ración, medio kilo, etc.
    }


    public enum TipoCuenta
    {
        Gratuita = 0,
        Freemium = 1,
        Profesional = 2,
        Empresarial = 3
    }

    public enum EstadoProducto
    {
        Activo = 0,
        Descontinuado = 1,
        Agotado = 2,
        BajoDemanda = 3
    }

    public enum TipoOferta
    {
        Ninguna = 0,
        Descuento = 1,
        DosPorUno = 2,
        Cupon = 3,
        Paquete = 4
    }

    public enum TipoIdentificacion
    {
        INE = 0,
        Pasaporte = 1,
        RFC = 2,
        CURP = 3,
        Otro = 4
    }

    /// <summary>
    /// Canal de distribución del proveedor.
    /// Define cómo hace llegar sus productos a los clientes/tiendas.
    /// </summary>
    public enum CanalProveedor
    {
        DirectoATienda = 0,     // Visita tiendas físicas y surte directamente (Ej: Coca-Cola, Sabritas)
        Mayorista = 1,          // Vende a granel a otras empresas o distribuidores
        Minorista = 2,          // Vende al por menor a consumidores finales
        Distribuidor = 3,       // Distribuye productos de otros proveedores
        PlataformaOnline = 4    // Opera vía apps/webs sin punto físico (dropshipping, e-commerce, etc.)
    }
}
