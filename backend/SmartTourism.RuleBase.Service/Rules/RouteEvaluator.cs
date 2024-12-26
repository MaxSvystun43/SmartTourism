using SmartTourism.RuleBase.Service.Models;

namespace SmartTourism.RuleBase.Service.Rules;

public class RouteEvaluator
{
    private const double MinRestaurantPosition = 0.4;
    private const double MaxRestaurantPosition = 0.8;

    public bool IsValidRoute(List<Point> route, double totalDistance)
    {
        // Rule: Only one restaurant
        var restaurantCount = route.Count(p => p.Categories.Contains("Restaurant"));
        if (restaurantCount != 1) return false;

        // Rule: Restaurant must be in the middle-to-end of the route
        var restaurantIndex = route.FindIndex(p => p.Categories.Contains("Restaurant"));
        var position = (double)restaurantIndex / route.Count;
        if (position < MinRestaurantPosition || position > MaxRestaurantPosition)
            return false;

        // Rule: Visit 50%-60% of points
        var visitedCount = route.Count;
        var totalPoints = route.Count; // or a predefined list
        if (visitedCount < totalPoints * 0.5 || visitedCount > totalPoints * 0.6)
            return false;

        return true;
    }
}
