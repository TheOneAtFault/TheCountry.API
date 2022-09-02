using TheCountry.API.EndpointConfigurations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointDefinitions(typeof(IEndpointDefinition));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            //https://localhost:7232
            policy.WithOrigins("https://thecountry.azurewebsites.net").AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});
builder.Services.AddMemoryCache();

var app = builder.Build();
app.UseEndpointDefinitions();
app.UseCors();
app.Run();