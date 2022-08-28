using System.Text.Json.Serialization;

namespace TheCountry.API.Models;

public class RegionModel
{

    [JsonPropertyName("name")]
    public Name? Name { get; set; }
    [JsonPropertyName("population")]
    public long Population { get; set; }
    [JsonPropertyName("subregion")]
    public string? SubRegion { get; set; }
}
