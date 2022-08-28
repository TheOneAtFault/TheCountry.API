using System.Text.Json.Serialization;

namespace TheCountry.API.Models
{
    public class Pager
    {
        [JsonPropertyName("total")]
        public int? Total { get; set; }
        [JsonPropertyName("currentpage")]
        public int? CurrentPage { get; set; }
        [JsonPropertyName("totalitemstoshow")]
        public int? TotalItemsToShow { get; set; }
        [JsonPropertyName("pagetotal")]
        public decimal? PageTotal { get; set; }

    }
}
