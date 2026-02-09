using AgendaPlus.Domain.Entities.Bases;

namespace AgendaPlus.Domain.Entities
{
    public class TenantAdress : BaseEntityReferenceTenant
    {
        public required string Cep { get; set; }
        public required string Street { get; set; }
        public string? Number { get; set; } = null;
        public string CountryCodeAlpha2 { get; set; } = "BR";
        public required string StateCode { get; set; }
        public required string CityName { get; set; }
        public decimal? Latitude { get; set; } = null;
        public decimal? Longitude { get; set; } = null;
        public string? GooglePlaceId { get; set; } = null;
    }
}
