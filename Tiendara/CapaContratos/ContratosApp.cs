// ------------------------------------------------------------
// Proyecto: Tiendara
// Autor: ZORRODEV
// Descripción: [Manejamos las interfaces en esta clase. Con el tiempo ira creciendo ZORRODEV - 2025]
// Fecha: [2025-08-19]
// Derechos reservados © ZORRODEV - 2025
// ------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaContratos
{

    //Interface para Auth o logins uwu con base de datos "{ZORRODEV-2025}"/
    public interface IAuthService
    {
        Task<Usuario> RegistrarAsync(string nombre, string apellidos, string email, string passwordPlano);
        Task<Usuario?> LoginAsync(string email, string passwordPlano);
    }


    public interface IProductoRepo : ICrudSql<Producto>
    {
        Task<List<Producto>> BuscarPorNombreAsync(string nombre);
        Task<List<Producto>> ListarPorCategoriaAsync(CategoriaProducto categoria);
    }


    // === CRUD SQL Genérico ===
    public interface IEntidadSql
    {
        Guid Id { get; set; }
    }

    public interface ICrudSql<T> where T : IEntidadSql
    {
        Task<T?> ObtenerPorIdAsync(Guid id);
        Task<List<T>> ObtenerTodosAsync();
        Task InsertarAsync(T entidad);
        Task ActualizarAsync(T entidad);
        Task EliminarAsync(Guid id);
    }

    // === INVENTARIO ===
    public interface IInventarioService
    {
        Task RegistrarEntrada(Guid negocioId, Guid productoId, decimal cantidad, decimal costoUnitario,
                              string? referencia = null, string? usuario = null);

        Task RegistrarSalida(Guid negocioId, Guid productoId, decimal cantidad,
                             Guid? ventaId = null, string? referencia = null, string? usuario = null);

        Task Ajustar(Guid negocioId, Guid productoId, decimal ajusteCantidad, string motivo, string? usuario = null);
    }

    // === CAJA ===
    public interface ICajaService
    {
        Task<Guid> RegistrarRetiro(Guid negocioId, decimal monto, string concepto,
                                   string? usuario = null, MedioPago medio = MedioPago.Efectivo);

        Task<CorteCaja> GenerarCorte(Guid negocioId, DateTime desde, DateTime hasta);
    }

    // === VENTAS ===
    public interface IVentaService
    {
        Task<Guid> RegistrarVentaAsync(IVendedor vendedor, SolicitudVenta s, CancellationToken ct = default);
    }

    public interface IVendedor
    {
        Guid UsuarioId { get; }
        bool PuedeVender { get; }

        Task<Guid> VenderAsync(SolicitudVenta solicitud, CancellationToken ct = default);
    }

    // === PRODUCTO / SERVICIO ===
    public interface IVendible
    {
        Guid Id { get; }
        string Nombre { get; }
        UnidadVenta UnidadVenta { get; }
        decimal PrecioBase { get; }
        decimal TasaImpuesto { get; }
        bool EsPesable { get; }
    }

    // === REPOSITORIOS BASE SQL ===
    public interface IUsuarioRepo
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByIdAsync(Guid id);
        Task AddAsync(Usuario u);
        Task UpdateAsync(Usuario u);
        Task<List<Usuario>> GetAllAsync();
    }

    public interface INegocioRepo 
    {
        Task AddAsync(Negocio n);
        Task UpdateAsync(Negocio n);
        Task<Negocio?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Negocio>> ListByUsuarioAsync(Guid propietarioId);
    }

    public interface IInventarioRepo : ICrudSql<Inventario>
    {
        Task<Inventario> GetOrCreateAsync(Guid negocioId, Guid productoId);
        Task AddMovimientoAsync(MovimientoInventario mov);
        Task<IReadOnlyList<MovimientoInventario>> ListMovimientosAsync(
            Guid negocioId, Guid productoId,
            DateTime? desde = null, DateTime? hasta = null);
    }


    public interface IVentaRepo
    {
        Task AddAsync(Venta v);
        Task<Venta?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Venta>> ListByFechaAsync(Guid negocioId, DateTime desde, DateTime hasta);
    }

    public interface ICajaRepo
    {
        Task AddMovimientoAsync(MovimientoCaja m);
        Task<IReadOnlyList<MovimientoCaja>> ListMovimientosAsync(Guid negocioId, DateTime desde, DateTime hasta);
    }

    public interface IMediaStorage
    {
        Task<string> SaveAsync(string fileName, Stream content, string contentType, CancellationToken ct = default);
        Task DeleteAsync(string url, CancellationToken ct = default);
    }
    // Nota: guarda binarios en file system/Blob y en BD solo URL + metadatos.


    //Interface para las publicaciones
    public interface IPublicacionRepo : ICrudSql<Publicacion>
    {
        Task<IReadOnlyList<Publicacion>> ListByAutorAsync(Guid autorId, bool esTienda, int top = 50);
        Task<IReadOnlyList<Publicacion>> FeedAsync(string country, string state, string city,
                                                   PublicationType? type = null, int top = 100);
    }
    //Interface para Chatrepo
    public interface IChatRepo
    {
        Task<Guid> EnsureThreadAsync(Guid remitenteId, bool remitenteEsTienda, Guid destinatarioId, bool destinatarioEsTienda);
        Task AddMensajeAsync(Guid threadId, Guid autorId, bool autorEsTienda, string texto, string? mediaUrl = null);
        Task<IReadOnlyList<Mensaje>> ListMensajesAsync(Guid threadId, int top = 100, DateTime? antesDe = null);
        Task<IReadOnlyList<ChatThread>> InboxAsync(Guid sujetoId, bool esTienda, int top = 50);
    }
    //Interface para cuando los usuarios se empiezen a seguir o tengan seguidores
    public interface ISeguimientoRepo
    {
        Task SeguirAsync(Guid seguidorUsuarioId, Guid targetId, bool targetEsTienda);
        Task DejarSeguirAsync(Guid seguidorUsuarioId, Guid targetId, bool targetEsTienda);
        Task<bool> SigueAsync(Guid seguidorUsuarioId, Guid targetId, bool targetEsTienda);
        Task<IReadOnlyList<Guid>> ListSeguidoresAsync(Guid targetId, bool targetEsTienda);
        Task<IReadOnlyList<Guid>> ListSiguiendoAsync(Guid usuarioId);
    }

    //Pagos despues con tarjetas
        public record PaymentResult(bool Ok, string? AuthorizationCode, string? Error);

        public interface IPaymentGateway
        {
            Task<PaymentResult> CobrarAsync(decimal monto, string concepto, CancellationToken ct = default);
        }
}
