using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Entidades;
using Tiendara.CapaDatos.Repos;
using Tiendara.CapaLogica.Servicios;

namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class RetirosPage : ContentPage
    {
        private readonly ICajaRepo _cajaRepo;
        private readonly IVentaRepo _ventaRepo;
        private readonly CajaService _cajaSvc;

        public ObservableCollection<RetiroItem> Items { get; } = new();

        public RetirosPage()
        {
            InitializeComponent();
            BindingContext = this;

            _cajaRepo = new CajaRepo();
            _ventaRepo = new VentaRepo();
            _cajaSvc = new CajaService(_cajaRepo, _ventaRepo);

            // Negocios disponibles (stub por ahora)
            pickNegocio.ItemsSource = Negocios;
            pickNegocio.ItemDisplayBinding = new Binding(nameof(NegocioStub.Nombre));
            pickNegocio.SelectedIndex = 0;

            _ = RefreshTotalsAsync();
        }

        private async void OnRegistrarClicked(object sender, EventArgs e)
        {
            try
            {
                if (pickNegocio.SelectedItem is not NegocioStub sel)
                {
                    await DisplayAlert("Retiros", "Selecciona un negocio.", "OK");
                    return;
                }

                if (!decimal.TryParse((txtMonto.Text ?? "0").Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var monto) || monto <= 0)
                {
                    await DisplayAlert("Retiros", "Monto inválido.", "OK");
                    return;
                }

                var motivo = (txtMotivo.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(motivo))
                {
                    await DisplayAlert("Retiros", "Escribe el motivo del retiro.", "OK");
                    return;
                }

                var referencia = (txtReferencia.Text ?? "").Trim();
                var usuario = "Dueño"; // TODO: enlazar al usuario actual

                // Registrar en servicio (MovimientoCaja con Tipo=Retiro)
                await _cajaSvc.RegistrarRetiro(
                    sel.Id,
                    monto,
                    concepto: string.IsNullOrWhiteSpace(referencia) ? motivo : $"{motivo} | Ref:{referencia}",
                    usuario: usuario
                );

                // Refrescar UI: prepend ítem
                Items.Insert(0, RetiroItem.From(sel.Nombre, monto, motivo, referencia, DateTime.Now));

                // Limpiar campos
                txtMonto.Text = string.Empty;
                txtMotivo.Text = string.Empty;
                txtReferencia.Text = string.Empty;

                await RefreshTotalsAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnResetSandboxClicked(object sender, EventArgs e)
        {
            try
            {
                // Borrar el archivo JSON de caja para "sandbox reset"
                var path = Path.Combine(FileSystem.AppDataDirectory, "caja.json");
                if (File.Exists(path)) File.Delete(path);

                Items.Clear();
                await RefreshTotalsAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task RefreshTotalsAsync()
        {
            // Sumamos retiros de todos los negocios stubs
            decimal totalHoy = 0m;
            decimal totalHistorico = 0m;

            var hoyDesde = DateTime.Today;
            var hoyHasta = hoyDesde.AddDays(1).AddTicks(-1);

            foreach (var n in Negocios)
            {
                var lHoy = await _cajaRepo.ListMovimientosAsync(n.Id, hoyDesde, hoyHasta);
                totalHoy += lHoy.Where(m => m.Tipo == TipoMovimientoCaja.Retiro).Sum(m => m.Monto);

                // Histórico: simple (rango amplio)
                var lAll = await _cajaRepo.ListMovimientosAsync(n.Id, DateTime.Today.AddYears(-20), DateTime.Today.AddYears(20));
                totalHistorico += lAll.Where(m => m.Tipo == TipoMovimientoCaja.Retiro).Sum(m => m.Monto);
            }

            lblRetirosHoy.Text = totalHoy.ToString("C", CultureInfo.CurrentCulture);
            lblRetirosTotal.Text = totalHistorico.ToString("C", CultureInfo.CurrentCulture);
        }

        // -------- ViewModel item para la lista --------
        public class RetiroItem
        {
            public string NegocioNombre { get; init; } = "";
            public string MontoTexto { get; init; } = "";
            public string Detalle { get; init; } = "";
            public string FechaCorta { get; init; } = "";

            public static RetiroItem From(string negocioNombre, decimal monto, string motivo, string referencia, DateTime fecha)
            {
                return new RetiroItem
                {
                    NegocioNombre = negocioNombre,
                    MontoTexto = monto.ToString("C", CultureInfo.CurrentCulture),
                    Detalle = string.IsNullOrWhiteSpace(referencia) ? motivo : $"{motivo}  Ref: {referencia}",
                    FechaCorta = fecha.ToString("dd/MM HH:mm")
                };
            }
        }

        // -------- Negocios "simples" por ahora --------
        private sealed class NegocioStub
        {
            public Guid Id { get; init; }
            public string Nombre { get; init; } = string.Empty;
        }

        private static readonly System.Collections.Generic.List<NegocioStub> Negocios = new()
        {
            new NegocioStub { Id = Guid.Empty, Nombre = "Caja general" }
            // Cuando tengas NegocioRepo poblado por usuario, reemplaza estos stubs.
        };
    }
}
