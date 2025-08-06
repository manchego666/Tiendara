// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [qué hace esta clase]
/// <summary>
/// Representa a un empleado dentro del sistema.
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
    public class Empleado : Persona
    {
        public decimal Salario { get; set; }
        public string Puesto { get; set; } // Cajero, Vendedor ,etc.
        public RolEmpleado Rol { get; set; }
    }

    public enum RolEmpleado
    {
        Cajero,
        Gerente,
        Cocinero
    }
}