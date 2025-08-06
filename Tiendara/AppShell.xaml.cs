namespace Tiendara
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("inventario", typeof(InventarioPage));
            Routing.RegisterRoute("venta", typeof(VentaPage));
            Routing.RegisterRoute("corte", typeof(CorteCajaPage));
            Routing.RegisterRoute("empleados", typeof(EmpleadosPage));
            Routing.RegisterRoute("proveedores", typeof(ProveedoresPage));
        }
    }
}