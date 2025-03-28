using System.Collections;

namespace Project_HealthChecker.Helpers.ClassExtensions;

public static class EnumerableExtension
{
    public static void Foreach(this IEnumerable enumerable, Action<object, int> action)
    {
        int currentElementNumber = 0;
        
        foreach (var element in enumerable)
            action?.Invoke(element, currentElementNumber);
    }
    
    public static void Foreach<TElement>(this IEnumerable<TElement> enumerable, Action<TElement, int> action)
    {
        int currentElementNumber = 0;
        
        foreach (var element in enumerable)
            action?.Invoke(element, currentElementNumber++);
    }
}