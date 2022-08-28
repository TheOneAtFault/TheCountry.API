using System.Text.Json.Serialization;

namespace TheCountry.API.Models
{
    public class CountryModel
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("currencies")]
        public List<string>? Currencies { get; set; }
        [JsonPropertyName("capital")]
        public List<string>? Capital { get; set; }
        [JsonPropertyName("region")]
        public string? Region { get; set; }
        [JsonPropertyName("subregion")]
        public string? Subregion { get; set; }
        [JsonPropertyName("languages")]
        public List<string>? Languages { get; set; }
        [JsonPropertyName("borders")]
        public List<string>? Borders { get; set; }
        [JsonPropertyName("population")]
        public long Population { get; set; }
    }
}
