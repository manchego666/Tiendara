using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiendara.CapaContratos;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaSql.Base;

namespace Tiendara.CapaLogica.Servicios
{
    public class NegocioService
    {
        private readonly INegocioRepo _negocios;

        public NegocioService(INegocioRepo negocios)
        {
            _negocios = negocios;
        }

        public async Task<Negocio?> GetByIdAsync(Guid id)
        {
            return await _negocios.GetByIdAsync(id);
        }

        public async Task CrearNegocioAsync(Negocio negocio)
        {
            if (negocio is null) throw new ArgumentNullException(nameof(negocio));
            await _negocios.AddAsync(negocio);
        }

        public async Task ActualizarDatosAsync(Guid id, string nombre, string direccion)
        {
            var n = await _negocios.GetByIdAsync(id);
            if (n is null) throw new InvalidOperationException("Negocio no encontrado");

            n.Nombre = nombre;
            n.Direccion = direccion;
            await _negocios.UpdateAsync(n);
        }

        public async Task EliminarNegocioAsync(Guid id)
        {
            var n = await _negocios.GetByIdAsync(id);
            if (n is null) throw new InvalidOperationException("Negocio no encontrado");

            await _negocios.EliminarAsync(n.Id);
        }
    }
}
