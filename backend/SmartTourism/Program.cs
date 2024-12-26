using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GeoApiService.Configuration;
using GeoApiService.Extensions;
using Serilog;
using SmartTourism.Database;
using SmartTourism.Endpoints;
using SmartTourism.RuleBase.Service.Services;
using SmartTourism.Services;

Console.OutputEncoding = Encoding.UTF8;

// Construct path dynamically in the "database" folder
string baseDirectory = "D:/study_3/GeoApi/backend/SmartTourism";
string databaseFolderPath = Path.Combine(baseDirectory, "database");

// Ensure the "database" folder exists
if (!Directory.Exists(databaseFolderPath))
{
    Directory.CreateDirectory(databaseFolderPath);
}

string databaseFilePath = Path.Combine(databaseFolderPath, "settings.db");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var configs = builder.Configuration.GetSection("GeoApiConfig").Get<HttpConfiguration>()
    ?? throw new ArgumentException("Configuration section [GeoApiConfig] not found");

builder.Services.AddGeoapifyApi(configs);

builder.Services.AddTransient<SmartTourismService>();
builder.Services.AddScoped<SettingsService>(provider => 
    new SettingsService(databaseFilePath));
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
app.AddSettingsEndpoints();
app.Run();