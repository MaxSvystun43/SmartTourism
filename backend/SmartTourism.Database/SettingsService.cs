
using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using SmartTourism.Database.Models;

namespace SmartTourism.Database;

public class SettingsService
{
    private readonly string _connectionString;

    public SettingsService(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS settings (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            VisitRestaurant BOOLEAN NOT NULL,
            LowerLimit INTEGER NOT NULL,
            UpperLimit INTEGER NOT NULL
        )";
        connection.Execute(createTableQuery);
    }

    // Insert a new setting
    public void AddSetting(Setting setting)
    {
        using var connection = new SqliteConnection(_connectionString);
        string insertQuery = @"
        INSERT INTO settings (VisitRestaurant, LowerLimit, UpperLimit)
        VALUES (@VisitRestaurant, @LowerLimit, @UpperLimit)";
        connection.Execute(insertQuery, setting);
    }

    // Get all settings
    public IEnumerable<Setting> GetAllSettings()
    {
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<Setting>("SELECT * FROM settings");
    }
    
    public Setting? GetLastSetting()
    {
        using var connection = new SqliteConnection(_connectionString);
        string lastRecordQuery = @"
        SELECT * FROM settings 
        ORDER BY Id DESC 
        LIMIT 1";
        return connection.QueryFirstOrDefault<Setting>(lastRecordQuery);
    }

    // Update a setting
    public void UpdateSetting(Setting setting)
    {
        using var connection = new SqliteConnection(_connectionString);
        string updateQuery = @"
        UPDATE settings 
        SET VisitRestaurant = @VisitRestaurant, 
            LowerLimit = @LowerLimit, 
            UpperLimit = @UpperLimit
        WHERE Id = @Id";
        connection.Execute(updateQuery, setting);
    }

    // Delete a setting by Id
    public void DeleteSetting(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        string deleteQuery = "DELETE FROM settings WHERE Id = @Id";
        connection.Execute(deleteQuery, new { Id = id });
    }
}
