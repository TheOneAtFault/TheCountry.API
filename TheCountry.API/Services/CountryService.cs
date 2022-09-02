using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using System.Text.Json;
using TheCountry.API.Models;
using TheCountry.API.Services;

namespace TheCountry.API.Repositories;

public class CountryService : ICountryService
{
    private readonly IMemoryCache _memoryCache;
    public CountryService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    public string GetAllCountries(string? search, int? page, int? getcount)
    {
        using (RestClient client = new RestClient("https://restcountries.com/v3.1"))
        {
            try
            {
                var pagnationCurrentPage = page ?? 1;
                var pagnationTotalItemsToGet = getcount ?? 15;

                List<CountryAPIModel> countryAPIMap = _memoryCache.Get<List<CountryAPIModel>>("CountryAPIMap");

                if (countryAPIMap == null)
                {
                    var request = new RestRequest("all");
                    request.AddParameter("fields", "name,population,capital,region,subregion,languages,currencies,borders");
                    var response = client.ExecuteGet<List<CountryAPIModel>>(request);
                    if (!response.IsSuccessful && response.Data == null)
                    {
                        return "";
                    }
                    countryAPIMap = response.Data;
                    _memoryCache.Set("CountryAPIMap", countryAPIMap, TimeSpan.FromMinutes(60));
                }

                var countryList = countryAPIMap?.Where(x=> (string.IsNullOrEmpty(search) || x.Name.Common.ToLower().Contains(search))).Select(item => new
                {
                    name = item.Name?.Common,
                    region = item.Region,
                    subregion = item.Subregion
                }).ToList();


                var payload = new
                {
                    countryList = countryList.OrderBy(o => o.name)
                    .Skip((pagnationCurrentPage - 1) * pagnationTotalItemsToGet)
                    .Take(pagnationTotalItemsToGet),
                    pagenation = new
                    {
                        currentPage = pagnationCurrentPage,
                        total = countryList?.Count,
                        totalItemsToShow = pagnationTotalItemsToGet,
                        pageTotal = Math.Ceiling(countryList.Count / (decimal)pagnationTotalItemsToGet)
                    }
                };

                return JsonSerializer.Serialize(payload);

            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
    public string GetCountry(string country)
    {
        using (RestClient client = new RestClient("https://restcountries.com/v3.1"))
        {
            try
            {
                // if the cache containing the entire country response is NOT empty
                // use that to filter out the result for the requested country
                // else only go and get the requested country and format the result.
                CountryAPIModel? countryAPIMap = _memoryCache.Get<List<CountryAPIModel>>("CountryAPIMap")?
                    .FirstOrDefault(x => x.Name?.Common == country);

                if (countryAPIMap == null)
                {
                    var request = new RestRequest($"name/{country}");
                    request.AddParameter("fields", "name,population,capital,region,subregion,languages,currencies,borders");
                    var response = client.ExecuteGet<List<CountryAPIModel>>(request);
                    if (!response.IsSuccessful || response.Data == null)
                    {
                        return "";
                    }
                    countryAPIMap = response.Data.FirstOrDefault();
                }

                var payload = new
                {
                    name = countryAPIMap?.Name?.Common,
                    capital = countryAPIMap?.Capital,
                    population = countryAPIMap?.Population,
                    languages = countryAPIMap?.Languages?.Values.ToList(),
                    currencies = countryAPIMap?.Currencies?.Values.Select(s=>s.Name).ToList(),
                    borders = countryAPIMap?.Borders
                };

                var countryJSON = JsonSerializer.Serialize(payload);

                return countryJSON;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
    public string GetRegion(string region)
    {
        using (RestClient client = new RestClient("https://restcountries.com/v3.1"))
        {
            try
            {
                // if the cache containing the entire country response is NOT empty
                // use that to filter out the result for the requested region
                // else only go and get the requested region and format the result.
                List<CountryAPIModel>? countryAPIMap = _memoryCache.Get<List<CountryAPIModel>>("CountryAPIMap")
                    .Where(x => x.Region.Equals(region)).ToList();

                if (countryAPIMap == null)
                {
                    var request = new RestRequest($"region/{region}");
                    request.AddParameter("fields", "name,population,subregion");
                    var response = client.ExecuteGet<List<CountryAPIModel>>(request);
                    if (!response.IsSuccessful)
                        return "";

                    countryAPIMap = response.Data;
                }

                var _region = new
                {
                    name = region,
                    population = countryAPIMap?.Sum(s => s.Population),
                    countryList = countryAPIMap?.Select(s => s.Name?.Common)
                    .OrderBy(o => o).ToList(),
                    subregionList = countryAPIMap?.GroupBy(g => g.Subregion)
                    .Select(x => x.First().Subregion).OrderBy(o => o).ToList()
                };

                return JsonSerializer.Serialize(_region);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
    public string GetSubregion(string region, string subregion)
    {
        using (RestClient client = new RestClient("https://restcountries.com/v3.1"))
        {
            try
            {
                // if the cache containing the entire country response is NOT empty
                // use that to filter out the result for the requested subregion
                // else go and request the country information belonging to the subregion.
                List<CountryAPIModel>? countryAPIMap = _memoryCache.Get<List<CountryAPIModel>>("CountryAPIMap")
                    .Where(x=>x.Subregion.Equals(subregion)).ToList();

                if (countryAPIMap == null)
                {
                    var request = new RestRequest($"subregion/{subregion}");
                    request.AddParameter("fields", "name,population,region");
                    var response = client.ExecuteGet<List<CountryAPIModel>>(request);
                    if (!response.IsSuccessful && response.Data != null)
                    {
                        return "";
                    }

                    countryAPIMap = response.Data;
                }

                var _subregion = new 
                {
                    name = subregion,
                    population = countryAPIMap?.Sum(s => s.Population),
                    countryList = countryAPIMap?.Select(s => s.Name?.Common).ToList(),
                    region = region
                };

                return JsonSerializer.Serialize(_subregion);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
