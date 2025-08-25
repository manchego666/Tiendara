using TiendaraMediaServer.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Escuchar en LAN
builder.WebHost.UseUrls("http://192.168.1.12:5080");

// Servicios
builder.Services.AddMediaServices(builder.Configuration);
builder.Services.AddCors();

var app = builder.Build();

// CORS abierto (ajústalo si quieres)
app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// /media => C:\Tiendara\Media
app.UseMediaStaticFiles(builder.Configuration);

// Endpoints
app.MapMediaEndpoints();

app.Run();
