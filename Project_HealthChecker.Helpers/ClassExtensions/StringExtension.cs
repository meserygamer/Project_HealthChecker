using System.Text.RegularExpressions;

namespace Project_HealthChecker.Helpers.ClassExtensions;

public static class StringExtension
{
    public static string CollapseSpaces(this string str) => 
        Regex.Replace(str, @"\s{2,}", " ");
}