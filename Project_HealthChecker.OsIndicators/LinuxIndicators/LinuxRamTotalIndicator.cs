using Project_HealthChecker.Helpers.ClassExtensions;
using Project_HealthChecker.OsIndicators.IndicatorInterfaces;

namespace Project_HealthChecker.OsIndicators.LinuxIndicators;

public class LinuxRamTotalIndicator : IRamTotalIndicator
{
    private const string PathToMemInfoFile = "/proc/meminfo";
    
    private const int BytesInKilobyte = 1024;
    
    public ulong GetRamTotal() => 
        GetRamTotalInKilobytes() * BytesInKilobyte;

    private ulong GetRamTotalInKilobytes()
    {
        using var memInfoStream = File.OpenText(PathToMemInfoFile);
        string ramTotalInfoLine = memInfoStream.ReadLine()!;
        return Convert.ToUInt64(
            ramTotalInfoLine
                .CollapseSpaces()
                .Split(" ")[1]);
    }
}