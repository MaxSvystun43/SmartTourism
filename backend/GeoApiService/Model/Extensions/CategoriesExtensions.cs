using GeoApiService.Model.Enums;

namespace GeoApiService.Model.Extensions;

internal static class CategoriesExtensions
{
    public static string ToSnakeString(this List<Category> categories)
    {
        var resultString = categories.ConvertAll(c => c.ToSnakeCase());

        var result = string.Join(",", resultString);
        
        return result;
    }
}