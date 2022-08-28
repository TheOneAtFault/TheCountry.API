using Microsoft.OpenApi.Models;
using TheCountry.API.EndpointConfigurations;

namespace TheCountry.API.EndpointDefinitions;

public class SwaggerEndpointDefinition : IEndpointDefinition
{
    public void DefineEndpoints(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShowMeTheCountryAPI v1"));
    }

    public void DefineServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShowMeTheCountry", Version = "1.0.0" });
        });
    }
}
