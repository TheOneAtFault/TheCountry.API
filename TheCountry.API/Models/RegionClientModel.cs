using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TheCountry.API.Models
{
    public class RegionClientModel
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("population")]
        public long Population { get; set; }
        [JsonPropertyName("countrylist")]
        public List<string>? CountryList { get; set; }
        [JsonPropertyName("subregionlist")]
        public List<string>? SubregionList { get; set; }
    }

}