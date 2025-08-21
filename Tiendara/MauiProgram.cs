using Microsoft.Extensions.Logging;
using Tiendara.CapaSql.Startup;
using Tiendara.CapaContratos;
using Tiendara.CapaLogica.Servicios;
using Tiendara.CapaSql.UsuarioRepo;
using Tiendara.CapaSql.ProductoRepo;
using Tiendara.CapaSql.InventarioRepo;
using Tiendara.CapaSql.VentaRepo;
using Tiendara.CapaSql.CajaRepo;
using Tiendara.CapaVisual.PaginasModulo;
using Tiendara.CapaVisual.Utils; // SessionService

namespace Tiendara;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.ClearProviders();
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

        //== REGISTRO centralizado de servicios sql
        builder.Services.AddTiendaraSql(
    "Server=localhost\\SQLEXPRESS;Database=TiendaraDB;User Id=TiendaraUser;Password=Tanshinie123;TrustServerCertificate=True;"
);

        // === DI de capa SQL / lógica (siempre, no solo en DEBUG) ===
        builder.Services.AddSingleton<IUsuarioRepo, UsuarioServiceSql>();
        builder.Services.AddSingleton<IProductoRepo, ProductoServiceSql>();
        builder.Services.AddSingleton<IInventarioRepo, InventarioServiceSql>();
        builder.Services.AddSingleton<IVentaRepo, VentaServiceSql>();
        builder.Services.AddSingleton<ICajaRepo, CajaServiceSql>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IVentaService, VentaService>();
        builder.Services.AddTransient<PerfilPage>();
        builder.Services.AddTransient<InventarioPage>();
        builder.Services.AddTransient<RetirosPage>();



        // Estado de sesión para toda la app
        builder.Services.AddSingleton<CapaVisual.Utils.SessionService>();

        var app = builder.Build();
        return app;

    }
}
