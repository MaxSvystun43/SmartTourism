using System.Text.RegularExpressions;

namespace GeoApiService.Model.Enums;

internal static class EnumConverter
{
    public static string ToSnakeCase(this Enum enumValue)
    {
        string enumString = enumValue.ToString();
        return Regex.Replace(enumString, "([a-z])([A-Z])", "$1_$2").ToLower();
    }
}