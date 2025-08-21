// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Clase base para todas las entidades SQL que no tienen otra herencia.
// Fecha: 2025-08-19
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Tiendara.CapaSql.Conexion
{
    public static class ConfiguracionSql
    {
        // DEV: PARA INSTANCIA REAL
        public static string ConnectionString { get; set; } =
            "Server=localhost\\SQLEXPRESS;Database=TiendaraDB;User Id=TiendaraUser;Password=Tanshinie123;TrustServerCertificate=True;";
    }
}
