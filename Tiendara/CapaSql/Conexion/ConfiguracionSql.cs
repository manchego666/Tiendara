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
          
        
        //La ruta para cargar el avatar o logo en los perfiles (solo se aceptara una foto por perfil ya sea negocio o usuario )
        //--NO ES RED SOCIAL-- despues se empezara a trabajar en seguridad para no depender de nubes ni rentar un host para pasar por link prefiero local en mi server...
        public static string MediaRootPath { get; } = @"C:\Tiendara\Media";
        public static string UsuariosAvatares => Path.Combine(MediaRootPath, "Usuarios", "Avatares");
        public static string NegociosLogos => Path.Combine(MediaRootPath, "Negocios", "Logos");
    }
}
