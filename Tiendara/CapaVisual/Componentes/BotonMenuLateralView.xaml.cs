using Microsoft.Maui.Controls;
using System;

namespace Tiendara.CapaVisual.Componentes
{
    public partial class BotonMenuLateralView : ContentView
    {
        public event EventHandler? Clicked;
        public BotonMenuLateralView() => InitializeComponent();
        private void OnClicked(object? s, EventArgs e) => Clicked?.Invoke(this, EventArgs.Empty);
    }
}
