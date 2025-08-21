// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Clase de usuario que pues pueden notar atributos para que tanto blablabla.[uwu]. Con el tiempo ira creciendo ZORRODEV - 2025]
// Fecha: [2025-08-10]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tiendara.CapaContratos;

namespace Tiendara.CapaDatos.Entidades;

public class Usuario : PersonaBase, IVendedor
{
    public HashSet<RolUsuario> Roles { get; } = new() { RolUsuario.UsuarioNormal };

    public Profesion? Profesion { get; set; }     // si es Profesionista
    public OficioTecnico? Oficio { get; set; }    // si es Tecnico

    // Datos de proveedor (opcionales)
    public FormaProveedor? FormaProveedor { get; set; }
    public EstatusMarca? EstadoMarca { get; set; }
    public string? NombreMarca { get; set; }

    public bool TieneRol(RolUsuario rol) => Roles.Contains(rol);
    public bool AgregarRol(RolUsuario rol) => Roles.Add(rol);
    public bool QuitarRol(RolUsuario rol) => Roles.Remove(rol);

    // ===== IVendedor =====
    public Guid UsuarioId => Id;
    public bool PuedeVender =>
        Roles.Contains(RolUsuario.Emprendedor) ||
        Roles.Contains(RolUsuario.Empleado) ||
        Roles.Contains(RolUsuario.Proveedor);

    // delegable por la capa lógica
    public Func<IVendedor, SolicitudVenta, CancellationToken, Task<Guid>>? VenderHandler { get; set; }

    public Task<Guid> VenderAsync(SolicitudVenta solicitud, CancellationToken ct = default)
    {
        if (!PuedeVender)
            throw new InvalidOperationException("Este usuario no tiene permisos para vender.");
        if (VenderHandler is null)
            throw new InvalidOperationException("No hay servicio de ventas asignado (VenderHandler es null).");

        return VenderHandler(this, solicitud, ct);
    }
}
