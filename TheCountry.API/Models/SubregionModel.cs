using System.Text.Json.Serialization;

namespace TheCountry.API.Models
{
    public class SubregionModel
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("population")]
        public long Population { get; set; }
        [JsonPropertyName("region")]
        public string? Region { get; set; }
        [JsonPropertyName("countrylist")]
        public List<string> CountryList { get; set; }

    }
}
