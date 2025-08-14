using Microsoft.Maui.Controls;

namespace Tiendara;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell(); // ← Asegura que usamos Shell
    }
}
