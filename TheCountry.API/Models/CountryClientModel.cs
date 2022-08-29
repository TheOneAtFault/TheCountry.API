using System.Text.Json.Serialization;

namespace TheCountry.API.Models
{
    public class CountryClientModel
    {
        [JsonPropertyName("countrylist")]
        public List<CountrySummaryModel>? CountryList { get; set; }
        [JsonPropertyName("pagenation")]
        public PagerModel? Pagenation { get; set; }
    }
}
