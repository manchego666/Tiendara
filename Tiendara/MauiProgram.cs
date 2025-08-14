using Microsoft.Extensions.Logging;

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

        // #if DEBUG
        // builder.Logging.AddDebug(); // quítala si no tienes el paquete
        // #endif

        return builder.Build();
    }
}
