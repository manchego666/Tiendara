using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Tiendara.CapaLogica.Infra;       // BackendConfig
using Tiendara.CapaLogica.Servicios;   // SessionService, FotoApiHttp
using Tiendara.CapaContratos;          // IFotoApi
using Tiendara.CapaVisual.PaginasModulo;
using Tiendara.CapaSql.Startup;
using Tiendara.CapaVisual.Autenticacion;

namespace Tiendara;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

#if DEBUG
        // URL del MediaServer en tu PC/LAN
        BackendConfig.BaseUrl = "http://192.168.1.12:5080";

        builder.Logging.ClearProviders();
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

        // ⚠️ Usa variable de entorno para no exponer password
        var conn = Environment.GetEnvironmentVariable("TIENDARA_SQL_CONN")
                   ?? "Server=localhost\\SQLEXPRESS;Database=TiendaraDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // TODO: cuando restaures Tiendara.CapaSql.Startup, llama aquí a AddTiendaraSql(conn)

        // DI básicos que ya tenías
        builder.Services.AddSingleton<SessionService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTiendaraSql(conn);
        builder.Services.AddTransient<PerfilPage>();
        builder.Services.AddTransient<PerfilNegocioPage>();
        builder.Services.AddTransient<RegistroTiendaPage>();
        builder.Services.AddTransient<RetirosPage>();
        builder.Services.AddTransient<RegistroTiendaPage>();
        builder.Services.AddTransient<InventarioPage>();





        // --- HttpClient nombrado "Api" para pegarle al MediaServer ---
        builder.Services.AddHttpClient("Api", c =>
        {
            c.BaseAddress = new Uri(BackendConfig.BaseUrl);
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        // Cliente de fotos (app -> MediaServer)
        builder.Services.AddSingleton<IFotoApi, FotoApiHttp>();

        return builder.Build();
    }
}
