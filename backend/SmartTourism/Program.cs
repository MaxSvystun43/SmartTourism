using System.Text;
using GeoApiService.Configuration;
using GeoApiService.Extensions;
using Serilog;
using SmartTourism.Endpoints;

Console.OutputEncoding = Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configs = builder.Configuration.GetSection("GeoApiConfig").Get<HttpConfiguration>()
    ?? throw new ArgumentException("Configuration section [GeoApiConfig] not found");

builder.Services.AddGeoapifyApi(configs);

builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.AddGeoEndpoints();
app.Run();