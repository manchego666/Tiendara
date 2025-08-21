// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: Clase base para todas las entidades SQL que no tienen otra herencia.
// Fecha: 2025-08-19
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------
using Tiendara.CapaContratos;
using System;
using Microsoft.Data.SqlClient;

namespace Tiendara.CapaSql.Base
{
    /// <summary>
    /// Clase base para entidades SQL. Implementa IEntidadSql y puede extenderse con lógica común.
    /// </summary>
    public abstract class EntidadSqlBase : IEntidadSql
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Método para generar un identificador único si aún no está definido.
        /// Puede usarse en lógica antes de insertar en base de datos.
        /// </summary>
        public void GenerarIdSiNoExiste()
        {
            if (Id == Guid.Empty)
                Id = Guid.NewGuid();
        }

        /// <summary>
        /// Devuelve una cadena de ID corta para debug/logs.
        /// </summary>
        public virtual string IdCorto()
        {
            return Id.ToString("N")[..8]; // primeros 8 caracteres
        }

        /// <summary>
        /// Retorna el nombre de la clase sin namespace.
        /// </summary>
        public override string ToString()
        {
            return $"{GetType().Name}[{IdCorto()}]";
        }
    }
}
