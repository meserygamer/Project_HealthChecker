using Project_HealthChecker.OsIndicators.IndicatorInterfaces;
using Project_HealthChecker.OsIndicators.LinuxIndicators;
using Project_HealthChecker.OsIndicators.WindowsIndicators;

namespace Project_HealthChecker.Server;

public static class ServiceCollectionExtension
{
    public static void AddOsIndicators(this IServiceCollection serviceCollection)
    {
        if (OperatingSystem.IsWindows())
        {
            serviceCollection.AddSingleton<IProcessorLoadIndicator, WindowsProcessorLoadIndicator>();
            
            return;
        }

        if (OperatingSystem.IsLinux())
        {
            serviceCollection.AddSingleton<IProcessorLoadIndicator, LinuxProcessorLoadIndicator>();
            
            return;
        }
    }
}