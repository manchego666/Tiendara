// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [qué hace esta clase]
/// <summary>
/// Clase base para representar una persona.
/// Puede ser usada para empleados, dueños, proveedores, etc.
/// </summary>
// Fecha: [2025-08-05]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiendara.CapaLogica
{
    public abstract class Persona
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; }
        public string City { get; set; } = string.Empty;
        public TipoPersona Tipo { get; set; }

    }

    public enum TipoPersona
    {
        Dueno,
        Proveedor,
        Empleado
    }
}