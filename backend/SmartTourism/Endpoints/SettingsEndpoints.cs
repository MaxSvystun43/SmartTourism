using Microsoft.AspNetCore.Mvc;
using SmartTourism.Database;
using SmartTourism.Database.Models;

namespace SmartTourism.Endpoints;

public static class SettingsEndpoints
{
    public static WebApplication AddSettingsEndpoints(this WebApplication app)
    {
        app.MapGet("/settings", (SettingsService settingsService) =>
        {
            // Retrieve and return the last inserted setting
            var lastSetting = settingsService.GetLastSetting();
            return Results.Ok(lastSetting);
        });
        
        app.MapPost("/add-settings", (
            SettingsService settingsService,
            [FromBody] Setting Settings
            ) =>
        {
            // Example of adding a new setting
            settingsService.AddSetting(Settings);

            // Retrieve and return the last inserted setting
            var lastSetting = settingsService.GetLastSetting();
            return Results.Ok(lastSetting);
        });
        
        return app;
    }
}