using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using TiendaraMediaServer.Contracts;

namespace TiendaraMediaServer.Endpoints;

public static class MediaEndpoints
{
    public static void MapMediaEndpoints(this IEndpointRouteBuilder app)
    {
        // Grupo para /api/media con antiforgery deshabilitado
        var media = app.MapGroup("/api/media")
                       .WithTags("Media")
                       .DisableAntiforgery();

        media.MapPost("usuario/{usuarioId:guid}/avatar", async (
            Guid usuarioId, IFormFile file, IFotoStorage storage, IFotoRepo repo) =>
        {
            if (file is null || file.Length == 0) return Results.BadRequest("Archivo vacío");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(ext)) return Results.BadRequest("jpg/png");

            await using var s = file.OpenReadStream();
            var rel = await storage.SaveAsync(MediaArea.UsuarioAvatar, usuarioId, s, ext);
            await repo.SetPerfilPathAsync(usuarioId, rel);
            return Results.Ok(new { relative = rel, url = storage.GetPublicUrl(rel) });
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        media.MapPost("negocio/{negocioId:guid}/logo", async (
            Guid negocioId, IFormFile file, IFotoStorage storage, IFotoRepo repo) =>
        {
            if (file is null || file.Length == 0) return Results.BadRequest("Archivo vacío");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(ext)) return Results.BadRequest("jpg/png");

            await using var s = file.OpenReadStream();
            var rel = await storage.SaveAsync(MediaArea.NegocioLogo, negocioId, s, ext);
            await repo.SetNegocioPathAsync(negocioId, rel);
            return Results.Ok(new { relative = rel, url = storage.GetPublicUrl(rel) });
        })
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }

    public static void UseMediaStaticFiles(this IApplicationBuilder app, IConfiguration cfg)
    {
        var root = cfg["Media:Root"] ?? @"C:\Tiendara\Media";
        var path = cfg["Media:BasePath"] ?? "/media";

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(root),
            RequestPath = path
        });
    }
}

public static class MediaServices
{
    public static IServiceCollection AddMediaServices(this IServiceCollection services, IConfiguration cfg)
    {
        var opt = new TiendaraMediaServer.Infrastructure.MediaOptions
        {
            Root = cfg["Media:Root"] ?? @"C:\Tiendara\Media",
            BasePath = cfg["Media:BasePath"] ?? "/media"
        };
        services.AddSingleton(opt);
        services.AddSingleton<TiendaraMediaServer.Contracts.IFotoStorage, TiendaraMediaServer.Infrastructure.FotoStorage>();

        var conn = cfg.GetConnectionString("Sql")
                   ?? Environment.GetEnvironmentVariable("TIENDARA_SQL_CONN")
                   ?? throw new InvalidOperationException("Connection string 'Sql' no configurada.");
        services.AddSingleton<TiendaraMediaServer.Contracts.IFotoRepo>(
            _ => new TiendaraMediaServer.Infrastructure.FotoRepoSql(conn));

        return services;
    }
}
