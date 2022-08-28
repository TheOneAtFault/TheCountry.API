using TheCountry.API.Repositories;
using TheCountry.API.EndpointConfigurations;
using System.Net.Http.Headers;
using TheCountry.API.Models;
using RestSharp;
using System.Text.Json;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using TheCountry.API.Services;

namespace TheCountry.API.EndpointDefinitions;

public class CountryEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.MapGet("/getallcountries", GetAllCountries);
        app.MapGet("/getcountry/{country}", GetCountry);
        app.MapGet("/getregion/{region}", GetRegion);
        app.MapGet("/getsubregion", GetSubregion);
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddSingleton<ICountryService, CountryService>();
    }

    internal IResult GetAllCountries(ICountryService service, string? search, int? page, int? getcount)
    {
        var response = service.GetAllCountries(search, page, getcount);
        return string.IsNullOrEmpty(response) ? Results.BadRequest() : Results.Ok(response);
    }

    internal IResult GetCountry(ICountryService service, string country)
    {
        if (string.IsNullOrEmpty(country))
            return Results.BadRequest();

        string response = service.GetCountry(country);
        return string.IsNullOrEmpty(response) ? Results.BadRequest() : Results.Ok(response);
    }

    internal IResult GetRegion(ICountryService service, string region)
    {
        if (string.IsNullOrEmpty(region))
            return Results.BadRequest();

        string response = service.GetRegion(region);
        return string.IsNullOrEmpty(response) ? Results.BadRequest() : Results.Ok(response);
    }

    internal IResult GetSubregion(ICountryService service, string region, string subregion)
    {
        if (string.IsNullOrEmpty(subregion))
            return Results.BadRequest();

        string response = service.GetSubregion(region, subregion);
        return string.IsNullOrEmpty(response) ? Results.BadRequest() : Results.Ok(response);
    }
}
