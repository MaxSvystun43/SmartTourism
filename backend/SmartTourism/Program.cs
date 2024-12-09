using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GeoApiService.Configuration;
using GeoApiService.Extensions;
using Serilog;
using SmartTourism.Endpoints;
using SmartTourism.RuleBase.Service.Services;
using SmartTourism.Services;

Console.OutputEncoding = Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var configs = builder.Configuration.GetSection("GeoApiConfig").Get<HttpConfiguration>()
    ?? throw new ArgumentException("Configuration section [GeoApiConfig] not found");

builder.Services.AddGeoapifyApi(configs);

builder.Services.AddTransient<SmartTourismService>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;	
    });

builder.Services.AddCors();

var app = builder.Build();


app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.AddGeoEndpoints();
app.Run();