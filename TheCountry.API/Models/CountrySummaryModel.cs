using System.Text.Json.Serialization;

namespace TheCountry.API.Models
{
    public class CountrySummaryModel
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("region")]
        public string? Region { get; set; }
        [JsonPropertyName("subregion")]
        public string? Subregion { get; set; }
        [JsonPropertyName("pagnation")]
        public PagerModel? Pagnation { get; set; }
    }
}
