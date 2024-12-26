using GeoApiService.Model.Enums;

namespace GeoApiService.Extensions;

internal static class GeoApiExtensions
{
    public static IReadOnlyList<Category> GetCategories(this List<string> categories)
    {
        var result = new List<Category>();

        foreach (var category in categories)
        {
            if (Enum.TryParse(category, true, out Category categoryEnum))
            {
                result.Add(categoryEnum);
            }
        }
        
        return result;
    }
}