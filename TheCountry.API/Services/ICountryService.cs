using System.Text.Json;

namespace TheCountry.API.Services
{
    public interface ICountryService
    {
        string GetAllCountries(string? search, int? page, int? getcount);
        string GetCountry(string country);
        string GetRegion(string region);
        string GetSubregion(string region, string subregion);
    }
}
