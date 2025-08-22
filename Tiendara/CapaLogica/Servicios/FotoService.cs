using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiendara.CapaSql.Conexion;

namespace Tiendara.CapaLogica.Servicios
{
    public class FotoService
    {
        public FotoService() { }

        public string GuardarFotoUsuario(byte[] foto, Guid usuarioId, string extension = ".jpg")
        {
            string ruta = ConfiguracionSql.UsuariosAvatares;

            if (!Directory.Exists(ruta))
                Directory.CreateDirectory(ruta);

            string fileName = $"{usuarioId}{extension}";
            string path = Path.Combine(ruta, fileName);

            File.WriteAllBytes(path, foto);

            return fileName; // Guardar solo el nombre en SQL
        }

        public string GuardarFotoNegocio(byte[] foto, Guid negocioId, string extension = ".jpg")
        {
            string ruta = ConfiguracionSql.NegociosLogos;

            if (!Directory.Exists(ruta))
                Directory.CreateDirectory(ruta);

            string fileName = $"{negocioId}{extension}";
            string path = Path.Combine(ruta, fileName);

            File.WriteAllBytes(path, foto);

            return fileName; // Guardar solo el nombre en SQL
        }
    }
}

