using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Tiendara.CapaDatos.Repos;
using Tiendara.CapaLogica.Servicios;

namespace Tiendara.CapaVisual.PaginasModulo
{
    public partial class InventarioPage : ContentPage
    {
        private readonly IInventarioRepo _repo;
        private readonly IInventarioService _svc;

        public InventarioPage()
        {
            InitializeComponent();
            _repo = new InventarioRepo();
            _svc = new InventarioService(_repo);
        }

        private Guid ParseGuidOrEmpty(string? s)
            => Guid.TryParse((s ?? string.Empty).Trim(), out var g) ? g : Guid.Empty;

        private decimal ParseDecimal(string? s)
            => decimal.TryParse((s ?? "0").Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var d) ? d : 0m;

        private async void OnEntradaClicked(object sender, EventArgs e)
        {
            try
            {
                var negocio = ParseGuidOrEmpty(txtNegocioIdE.Text);
                var producto = Guid.Parse((txtProductoIdE.Text ?? "").Trim());
                var cantidad = ParseDecimal(txtCantidadE.Text);
                var costo = ParseDecimal(txtCostoE.Text);
                if (cantidad <= 0 || costo < 0)
                {
                    await DisplayAlert("Inventario", "Cantidad/costo inválidos.", "OK");
                    return;
                }
                await _svc.RegistrarEntrada(negocio, producto, cantidad, costo, referencia: txtRefE.Text, usuario: "sandbox");
                await DisplayAlert("Inventario", "Entrada registrada.", "OK");
            }
            catch (Exception ex) { await DisplayAlert("Error", ex.Message, "OK"); }
        }

        private async void OnSalidaClicked(object sender, EventArgs e)
        {
            try
            {
                var negocio = ParseGuidOrEmpty(txtNegocioIdS.Text);
                var producto = Guid.Parse((txtProductoIdS.Text ?? "").Trim());
                var cantidad = ParseDecimal(txtCantidadS.Text);
                if (cantidad <= 0)
                {
                    await DisplayAlert("Inventario", "Cantidad inválida.", "OK");
                    return;
                }
                await _svc.RegistrarSalida(negocio, producto, cantidad, referencia: txtRefS.Text, usuario: "sandbox");
                await DisplayAlert("Inventario", "Salida registrada.", "OK");
            }
            catch (Exception ex) { await DisplayAlert("Error", ex.Message, "OK"); }
        }

        private async void OnAjusteClicked(object sender, EventArgs e)
        {
            try
            {
                var negocio = ParseGuidOrEmpty(txtNegocioIdA.Text);
                var producto = Guid.Parse((txtProductoIdA.Text ?? "").Trim());
                var ajuste = ParseDecimal(txtAjusteA.Text);
                if (ajuste == 0)
                {
                    await DisplayAlert("Inventario", "Escribe un ajuste (positivo o negativo).", "OK");
                    return;
                }
                var motivo = (txtMotivoA.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(motivo)) motivo = "Ajuste";
                await _svc.Ajustar(negocio, producto, ajuste, motivo, usuario: "sandbox");
                await DisplayAlert("Inventario", "Ajuste aplicado.", "OK");
            }
            catch (Exception ex) { await DisplayAlert("Error", ex.Message, "OK"); }
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            // Aquí refrescarías una lista si la agregas. De momento es un no-op.
            await Task.CompletedTask;
        }
    }
}
