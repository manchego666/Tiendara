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
        Tiendaro = 0,    // comerciante/cliente base que usa la app para su tienda
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
    /// Unidad de venta para productos/servicios.
    /// </summary>
    public enum UnidadVenta
    {
        Pza = 0,
        Kg = 1,
        Lt = 2,
        Ml = 3,
        G = 4,
        Servicio = 5
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

    public enum EstadoVenta { Abierta = 0, Finalizada = 1, Cancelada = 2 }

    public enum MedioPago { Efectivo = 0, Tarjeta = 1, Transferencia = 2, Mixto = 3 }

    public enum TipoMovimientoCaja { IngresoVenta = 0, Retiro = 1, Ajuste = 2 }
}
    ////  luego distinguir canal comercial del proveedor:
    //public enum CanalProveedor
    //{
    //    DirectoATienda = 0,   // visita y surte tiendas (p. ej., Coca, Sabritas)
    //    Mayorista = 1,
    //    Minorista = 2,
    //    Distribuidor = 3
    //}
