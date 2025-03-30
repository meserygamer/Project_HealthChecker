using System.Diagnostics.CodeAnalysis;
using System.Management;
using Project_HealthChecker.OsIndicators.IndicatorInterfaces;

namespace Project_HealthChecker.OsIndicators.WindowsIndicators;

[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
public class WindowsRamTotalIndicator : IRamTotalIndicator
{
    private const string QueryMemoryProperty = "TotalPhysicalMemory";
    
    private const string TotalPhysicalMemoryWmiQuery = $"SELECT {QueryMemoryProperty} FROM Win32_ComputerSystem";
    
    public ulong GetRamTotal()
    {
        using var searcher = new ManagementObjectSearcher(TotalPhysicalMemoryWmiQuery);
        foreach (var obj in searcher.Get())
        {
            var totalMemory = (ulong)obj[QueryMemoryProperty];
            return totalMemory;
        }

        throw new ApplicationException($"{QueryMemoryProperty} WMI Query not returned result!");
    }
}