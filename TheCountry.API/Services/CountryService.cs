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

                var finalCountryList = countryAPIMap.Select(item => new CountrySummaryModel
                {
                    Name = item.Name?.Common,
                    Region = item.Region,
                    Subregion = item.Subregion
                });

                if (!string.IsNullOrEmpty(search))
                    finalCountryList = finalCountryList.Where(p => p.Name.ToLower().Contains(search));

                PagerModel pager = new PagerModel()
                {
                    CurrentPage = pagnationCurrentPage,
                    Total = finalCountryList.ToList().Count,
                    TotalItemsToShow = pagnationTotalItemsToGet,
                    PageTotal = Math.Ceiling(finalCountryList.ToList().Count / (decimal)pagnationTotalItemsToGet)
                };

                CountryClientModel countryClient = new CountryClientModel()
                {
                    CountryList = finalCountryList.OrderBy(o => o.Name)
                            .Skip((pagnationCurrentPage - 1) * pagnationTotalItemsToGet)
                            .Take(pagnationTotalItemsToGet).ToList(),
                    Pagenation = pager
                };

                return JsonSerializer.Serialize(countryClient);

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
                    .FirstOrDefault(x => x.Name.Common == country);

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

                // format the result to make it easier for the client.
                var languageList = new List<string>();
                foreach (var language in countryAPIMap.Languages)
                {
                    languageList.Add(language.Value);
                }

                var currencyList = new List<string>();
                foreach (var currency in countryAPIMap.Currencies)
                {
                    currencyList.Add(currency.Value.Name);
                }

                CountryModel countryFull = new CountryModel()
                {
                    Name = countryAPIMap.Name.Common,
                    Capital = countryAPIMap.Capital,
                    Population = countryAPIMap.Population,
                    Languages = languageList,
                    Currencies = currencyList,
                    Borders = countryAPIMap.Borders
                };

                var countryJSON = JsonSerializer.Serialize(countryFull);

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
                List<CountryAPIModel>? countryAPIMap = _memoryCache.Get<List<CountryAPIModel>>("CountryAPIMap");
                List<RegionModel> regionCountries = new();

                if (countryAPIMap == null)
                {
                    var request = new RestRequest($"region/{region}");
                    request.AddParameter("fields", "name,population,subregion");
                    var response = client.ExecuteGet<List<RegionModel>>(request);
                    if (!response.IsSuccessful)
                        return "";

                    regionCountries = response.Data;
                }
                else
                {
                    regionCountries = countryAPIMap.Where(x => x.Region == region)
                        .Select(s => new RegionModel { Population = s.Population, Name = s.Name, SubRegion = s.Subregion }).ToList();
                }

                if (regionCountries == null || regionCountries.Count == 0)
                    return "";

                RegionClientModel _region = new RegionClientModel()
                {
                    Name = region,
                    Population = regionCountries.Sum(s => s.Population),
                    CountryList = regionCountries.Select(x => x.Name.Common).OrderBy(o => o).ToList(),
                    SubregionList = regionCountries.GroupBy(g => g.SubRegion).Select(x => x.First().SubRegion).OrderBy(o => o).ToList()
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
                List<CountryAPIModel>? countryAPIMap = _memoryCache.Get<List<CountryAPIModel>>("CountryAPIMap");
                List<RegionModel> subregionList = new();

                if (countryAPIMap == null)
                {
                    var request = new RestRequest($"subregion/{subregion}");
                    request.AddParameter("fields", "name,population,region");
                    var response = client.ExecuteGet<List<RegionModel>>(request);
                    if (!response.IsSuccessful && response.Data != null)
                    {
                        return "";
                    }

                    subregionList = response.Data;
                }
                else
                {
                    subregionList = countryAPIMap.Where(x => x.Subregion == subregion)
                        .Select(s => new RegionModel { Population = s.Population, Name = s.Name, SubRegion = s.Subregion }).ToList();
                }

                SubregionModel _subregion = new SubregionModel()
                {
                    Name = subregion,
                    Population = subregionList.Sum(s => s.Population),
                    CountryList = subregionList.Select(s=>s.Name.Common).ToList(),
                    Region = region
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
