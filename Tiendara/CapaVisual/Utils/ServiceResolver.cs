using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace Tiendara.CapaVisual.Utils
{
    public static class ServiceResolver
    {
        private static IServiceProvider Provider =>
            Application.Current?.Handler?.MauiContext?.Services
            ?? throw new InvalidOperationException("DI no disponible (MauiContext.Services es null).");

        public static T Get<T>() where T : notnull => Provider.GetRequiredService<T>();
        public static T? TryGet<T>() where T : class => Provider.GetService<T>();
    }
}
