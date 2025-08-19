// ------------------------------------------------------------
// Proyecto: Tiendara
// Archivo: Ubicacion.cs
// ------------------------------------------------------------
namespace Tiendara.CapaDatos.Entidades
{
    public class Ubicacion
    {
        public string Country { get; set; } = "MX";
        public string State { get; set; } = "Sinaloa";
        public string City { get; set; } = "Culiacán";

        public override string ToString() => $"{Country} · {State} · {City}";
    }
}
