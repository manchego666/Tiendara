using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiendara.CapaSql;

public class ConfiguracionSql
{
    public string Servidor { get; set; } = "localhost\\SQLEXPRESS";
    public string Usuario { get; set; } = "TiendaraUser"; //Cambiara Despues
    public string Password { get; set; } = "Tanshinie123"; // Cambiara despues ZDEV - 2025

    public string BaseDeDatos { get; set; } = "TiendaraDB";

    public string GetConnectionString(bool incluirDB = true)
    {
        var db = incluirDB ? $"Database={BaseDeDatos};" : "";
        return $"Server={Servidor};{db}User Id={Usuario};Password={Password};TrustServerCertificate=True;";
    }

    public string GetMasterConnectionString()
    {
        return $"Server={Servidor};Database=master;User Id={Usuario};Password={Password};TrustServerCertificate=True;";
    }
}
