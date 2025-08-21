using Microsoft.Extensions.DependencyInjection;
using Tiendara.CapaContratos;
using Tiendara.CapaSql.CajaRepo;
using Tiendara.CapaSql.ChatRepo;
using Tiendara.CapaSql.Conexion;
using Tiendara.CapaSql.Infra;
using Tiendara.CapaSql.InventarioRepo;
using Tiendara.CapaSql.NegocioRepo;
using Tiendara.CapaSql.ProductoRepo;
using Tiendara.CapaSql.PublicacionRepo;
using Tiendara.CapaSql.SeguimientoRepo;
using Tiendara.CapaSql.UsuarioRepo;
using Tiendara.CapaLogica.Servicios;
using Tiendara.CapaVisual.PaginasModulo;

namespace Tiendara.CapaSql.Startup
{
    public static class TiendaraSqlRegistration
    {
        public static IServiceCollection AddTiendaraSql(
            this IServiceCollection services,
            string connectionString,
            string? mediaRoot = null)
        {
            ConfiguracionSql.ConnectionString = connectionString;

            services.AddSingleton<IUsuarioRepo, UsuarioServiceSql>();
            services.AddSingleton<IProductoRepo, ProductoServiceSql>();
            services.AddSingleton<IInventarioRepo, InventarioServiceSql>();
            services.AddSingleton<IVentaRepo, VentaRepo.VentaServiceSql>();
            services.AddSingleton<ICajaRepo, CajaServiceSql>();
            services.AddSingleton<IPublicacionRepo, PublicacionServiceSql>();
            services.AddSingleton<IChatRepo, ChatServiceSql>();
            services.AddSingleton<INegocioRepo, NegocioServiceSql>();
            services.AddSingleton<ISeguimientoRepo, SeguimientoServiceSql>();
            services.AddSingleton<SessionService>();
            services.AddTransient<PerfilPage>();


            if (!string.IsNullOrWhiteSpace(mediaRoot))
                services.AddSingleton<IMediaStorage>(new FileSystemMediaStorage(mediaRoot));

            return services;
        }
    }
}
