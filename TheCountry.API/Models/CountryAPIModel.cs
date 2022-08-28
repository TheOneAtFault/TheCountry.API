using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TheCountry.API.Models;

public class CountryAPIModel
{
    [JsonPropertyName("name")]
    public Name? Name { get; set; }
    [JsonPropertyName("currencies")]
    public Dictionary<string, Currency>? Currencies { get; set; }
    [JsonPropertyName("capital")]
    public List<string>? Capital { get; set; }
    [JsonPropertyName("region")]
    public string? Region { get; set; }
    [JsonPropertyName("subregion")]
    public string? Subregion { get; set; }
    [JsonPropertyName("languages")]
    public Dictionary<string, string>? Languages { get; set; }
    [JsonPropertyName("borders")]
    public List<string>? Borders { get; set; }
    [JsonPropertyName("population")]
    public long Population { get; set; }
}

public class Name
{
    [JsonPropertyName("common")]
    public string? Common { get; set; }
}

public class Currency
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
}
