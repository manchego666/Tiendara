// ------------------------------------------------------------
// Proyecto: Tiendara
// Archivo: CapaDatos/Repos/InterfacesRepos.cs
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;                 // <- para FileSystem.AppDataDirectory
using Tiendara.CapaDatos.Entidades;

namespace Tiendara.CapaDatos.Repos;

// ===== Base JSON store =====
public abstract class JsonRepoBase<T> where T : class
{
    protected readonly string FilePath;
    protected readonly JsonSerializerOptions Opts = new JsonSerializerOptions { WriteIndented = true };

    protected JsonRepoBase(string fileName)
    {
        var dir = FileSystem.AppDataDirectory;
        FilePath = Path.Combine(dir, fileName);
        if (!File.Exists(FilePath))
            File.WriteAllText(FilePath, "[]");
    }

    protected async Task<List<T>> LoadAsync()
    {
        using var s = File.OpenRead(FilePath);
        var list = await JsonSerializer.DeserializeAsync<List<T>>(s, Opts) ?? new List<T>();
        return list;
    }

    protected async Task SaveAsync(List<T> items)
    {
        using var s = File.Create(FilePath);
        await JsonSerializer.SerializeAsync(s, items, Opts);
    }
}

// ===== Usuarios =====
public interface IUsuarioRepo
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByIdAsync(Guid id);
    Task AddAsync(Usuario u);
    Task UpdateAsync(Usuario u);
    Task<List<Usuario>> GetAllAsync();
}

public sealed class UsuarioRepo : JsonRepoBase<Usuario>, IUsuarioRepo
{
    public UsuarioRepo() : base("usuarios.json") { }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        email = (email ?? "").Trim().ToLowerInvariant();
        return (await LoadAsync()).FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Usuario?> GetByIdAsync(Guid id)
        => (await LoadAsync()).FirstOrDefault(u => u.Id == id);

    public async Task AddAsync(Usuario u)
    {
        var list = await LoadAsync();
        list.Add(u);
        await SaveAsync(list);
    }

    public async Task UpdateAsync(Usuario u)
    {
        var list = await LoadAsync();
        var idx = list.FindIndex(x => x.Id == u.Id);
        if (idx >= 0) list[idx] = u;
        await SaveAsync(list);
    }

    public async Task<List<Usuario>> GetAllAsync() => await LoadAsync();
}

// ===== Negocios =====
public interface INegocioRepo
{
    Task AddAsync(Negocio n);
    Task UpdateAsync(Negocio n);
    Task<Negocio?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Negocio>> ListByUsuarioAsync(Guid propietarioId);
}

public sealed class NegocioRepo : JsonRepoBase<Negocio>, INegocioRepo
{
    public NegocioRepo() : base("negocios.json") { }

    public async Task AddAsync(Negocio n)
    {
        var list = await LoadAsync();
        list.Add(n);
        await SaveAsync(list);
    }

    public async Task UpdateAsync(Negocio n)
    {
        var list = await LoadAsync();
        var idx = list.FindIndex(x => x.Id == n.Id);
        if (idx >= 0) list[idx] = n;
        await SaveAsync(list);
    }

    public async Task<Negocio?> GetByIdAsync(Guid id)
        => (await LoadAsync()).FirstOrDefault(x => x.Id == id);

    public async Task<IReadOnlyList<Negocio>> ListByUsuarioAsync(Guid propietarioId)
        => (await LoadAsync()).Where(x => x.PropietarioUsuarioId == propietarioId).ToList();
}

// ===== Inventario + Movimientos =====
public interface IInventarioRepo
{
    Task<Inventario> GetOrCreateAsync(Guid negocioId, Guid productoId);
    Task UpdateAsync(Inventario inv);
    Task AddMovimientoAsync(MovimientoInventario mov);
    Task<IReadOnlyList<MovimientoInventario>> ListMovimientosAsync(Guid negocioId, Guid productoId,
        DateTime? desde = null, DateTime? hasta = null);
}

public sealed class InventarioRepo : JsonRepoBase<Inventario>, IInventarioRepo
{
    private readonly string movPath;
    private readonly JsonSerializerOptions opt = new() { WriteIndented = true };

    public InventarioRepo() : base("inventarios.json")
    {
        var dir = FileSystem.AppDataDirectory;
        movPath = Path.Combine(dir, "mov_inventario.json");
        if (!File.Exists(movPath)) File.WriteAllText(movPath, "[]");
    }

    private async Task<List<MovimientoInventario>> LoadMovs()
    {
        using var s = File.OpenRead(movPath);
        return await JsonSerializer.DeserializeAsync<List<MovimientoInventario>>(s, opt) ?? new();
    }

    private async Task SaveMovs(List<MovimientoInventario> m)
    {
        using var s = File.Create(movPath);
        await JsonSerializer.SerializeAsync(s, m, opt);
    }

    public async Task<Inventario> GetOrCreateAsync(Guid negocioId, Guid productoId)
    {
        var list = await LoadAsync();
        var inv = list.FirstOrDefault(x => x.NegocioId == negocioId && x.ProductoId == productoId);
        if (inv is null)
        {
            inv = new Inventario { NegocioId = negocioId, ProductoId = productoId };
            list.Add(inv);
            await SaveAsync(list);
        }
        return inv;
    }

    public async Task UpdateAsync(Inventario inv)
    {
        var list = await LoadAsync();
        var idx = list.FindIndex(x => x.Id == inv.Id);
        if (idx >= 0) list[idx] = inv;
        await SaveAsync(list);
    }

    public async Task AddMovimientoAsync(MovimientoInventario mov)
    {
        var list = await LoadMovs();
        list.Add(mov);
        await SaveMovs(list);
    }

    public async Task<IReadOnlyList<MovimientoInventario>> ListMovimientosAsync(Guid negocioId, Guid productoId,
        DateTime? desde = null, DateTime? hasta = null)
    {
        var list = await LoadMovs();
        var q = list.Where(x => x.NegocioId == negocioId && x.ProductoId == productoId);
        if (desde is not null) q = q.Where(x => x.Fecha >= desde.Value);
        if (hasta is not null) q = q.Where(x => x.Fecha <= hasta.Value);
        return q.OrderBy(x => x.Fecha).ToList();
    }
}

// ===== Ventas =====
public interface IVentaRepo
{
    Task AddAsync(Venta v);
    Task<Venta?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Venta>> ListByFechaAsync(Guid negocioId, DateTime desde, DateTime hasta);
}

public sealed class VentaRepo : JsonRepoBase<Venta>, IVentaRepo
{
    public VentaRepo() : base("ventas.json") { }

    public async Task AddAsync(Venta v)
    {
        var list = await LoadAsync();
        list.Add(v);
        await SaveAsync(list);
    }

    public async Task<Venta?> GetByIdAsync(Guid id)
        => (await LoadAsync()).FirstOrDefault(x => x.Id == id);

    public async Task<IReadOnlyList<Venta>> ListByFechaAsync(Guid negocioId, DateTime desde, DateTime hasta)
        => (await LoadAsync()).Where(x => x.NegocioId == negocioId && x.Fecha >= desde && x.Fecha <= hasta).ToList();
}

// ===== Caja =====
public interface ICajaRepo
{
    Task AddMovimientoAsync(MovimientoCaja m);
    Task<IReadOnlyList<MovimientoCaja>> ListMovimientosAsync(Guid negocioId, DateTime desde, DateTime hasta);
}

public sealed class CajaRepo : JsonRepoBase<MovimientoCaja>, ICajaRepo
{
    public CajaRepo() : base("caja.json") { }

    public async Task AddMovimientoAsync(MovimientoCaja m)
    {
        var list = await LoadAsync();
        list.Add(m);
        await SaveAsync(list);
    }

    public async Task<IReadOnlyList<MovimientoCaja>> ListMovimientosAsync(Guid negocioId, DateTime desde, DateTime hasta)
        => (await LoadAsync()).Where(x => x.NegocioId == negocioId && x.Fecha >= desde && x.Fecha <= hasta).ToList();
}
