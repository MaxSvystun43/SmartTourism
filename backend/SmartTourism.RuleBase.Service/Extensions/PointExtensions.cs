using SmartTourism.RuleBase.Service.Models;

namespace SmartTourism.RuleBase.Service.Extensions;

public static class PointExtensions
{
    const double R = 6371e3; // Earth radius in meters
    
    public static double Heuristic(this Point current, Point target)
    {
        return CalculateDistance(current.Latitude, current.Longitude, target.Latitude, target.Longitude);
    }

    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var f1 = DegreesToRadians(lat1);
        var f2 = DegreesToRadians(lat2);
        var deltaLat = DegreesToRadians(lat2 - lat1);
        var deltaLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(f1) * Math.Cos(f2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Distance in meters
    }
    
    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
}