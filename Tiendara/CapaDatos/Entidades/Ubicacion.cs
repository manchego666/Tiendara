// ------------------------------------------------------------
// Proyecto: Tiendara
// Archivo: Ubicacion.cs (Value Object, sin Id)
// Autor: ZDEV - 2025
// Todos los derechos reservados.
// Fecha : 20/08/2025
// ------------------------------------------------------------
namespace Tiendara.CapaDatos.Entidades
{
    public sealed class Ubicacion
    {
        // País
        public string? Country { get; set; }          // "Mexico"
        public string? CountryCode { get; set; }      // "MX" (ISO-3166-1 alpha-2)

        // Estado/Provincia
        public string? State { get; set; }            // "Sinaloa"
        public string? StateCode { get; set; }        // "SIN" (si usas abreviado)

        // Ciudad/Localidad/Colonia
        public string? City { get; set; }             // "Culiacán"
        public string? Locality { get; set; }         // colonia/barrio opcional

        // Dirección y CP
        public string? AddressLine1 { get; set; }     // calle y número
        public string? AddressLine2 { get; set; }     // interior, referencia
        public string? PostalCode { get; set; }       // "80000"

        // Geoposición (opcional)
        public double? Lat { get; set; }
        public double? Lng { get; set; }

        // Zona horaria (opcional, IANA)
        public string? TimeZone { get; set; }         // "America/Mazatlan"

        public void Normalize()
        {
            CountryCode = string.IsNullOrWhiteSpace(CountryCode) ? null : CountryCode.Trim().ToUpperInvariant();
            StateCode = string.IsNullOrWhiteSpace(StateCode) ? null : StateCode.Trim().ToUpperInvariant();
            PostalCode = string.IsNullOrWhiteSpace(PostalCode) ? null : PostalCode.Trim();
            Country = string.IsNullOrWhiteSpace(Country) ? null : Country.Trim();
            State = string.IsNullOrWhiteSpace(State) ? null : State.Trim();
            City = string.IsNullOrWhiteSpace(City) ? null : City.Trim();
            Locality = string.IsNullOrWhiteSpace(Locality) ? null : Locality.Trim();
            AddressLine1 = string.IsNullOrWhiteSpace(AddressLine1) ? null : AddressLine1.Trim();
            AddressLine2 = string.IsNullOrWhiteSpace(AddressLine2) ? null : AddressLine2.Trim();
            TimeZone = string.IsNullOrWhiteSpace(TimeZone) ? null : TimeZone.Trim();
        }

        public bool IsEmpty()
            => string.IsNullOrWhiteSpace(Country)
            && string.IsNullOrWhiteSpace(State)
            && string.IsNullOrWhiteSpace(City)
            && string.IsNullOrWhiteSpace(AddressLine1)
            && string.IsNullOrWhiteSpace(PostalCode)
            && Lat is null && Lng is null;

        public override string ToString()
        {
            // Construye una vista amigable (evita " ·  · ")
            var parts = new List<string?>();
            if (!string.IsNullOrWhiteSpace(AddressLine1)) parts.Add(AddressLine1);
            if (!string.IsNullOrWhiteSpace(City)) parts.Add(City);
            if (!string.IsNullOrWhiteSpace(State)) parts.Add(State);
            if (!string.IsNullOrWhiteSpace(CountryCode)) parts.Add(CountryCode);
            else if (!string.IsNullOrWhiteSpace(Country)) parts.Add(Country);
            return string.Join(" · ", parts);
        }
    }
}
