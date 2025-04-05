using System.Diagnostics.CodeAnalysis;
using System.Management;
using Project_HealthChecker.OsIndicators.BaseClasses;
using Project_HealthChecker.OsIndicators.IndicatorInterfaces;

namespace Project_HealthChecker.OsIndicators.WindowsIndicators;

[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
public class WindowsRamUsingIndicator : BaseChangingOverTimeIndicator, IRamUsingIndicator
{
    private const string TotalMemoryProperty = "TotalPhysicalMemory";
    private const string FreeMemoryProperty = "FreePhysicalMemory";

    private const string TotalAndFreeMemoryWmiQuery = $"SELECT " +
        $"{TotalMemoryProperty}" +
        $",{FreeMemoryProperty} " +
        $"FROM Win32_ComputerSystem";

    private Task? _indicationTask;

    private CancellationTokenSource? _indicationTaskCancellationTokenSource;

    public ulong UsingMemory { get; private set; }
    
    public override void Start()
    {
        if (_indicationTaskCancellationTokenSource is not null)
            return;

        _indicationTaskCancellationTokenSource = new CancellationTokenSource();
        var token = _indicationTaskCancellationTokenSource.Token;
        _indicationTask = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                UsingMemory = GetOccupiedMemory();
                await Task.Delay(base.MeasurementInterval, token);
            }
        }, token);
    }

    public override void Pause()
    {
        if (_indicationTaskCancellationTokenSource is null)
            return;

        _indicationTaskCancellationTokenSource.Cancel();
        _indicationTaskCancellationTokenSource = null;
        _indicationTask = null;
    }

    private ulong GetOccupiedMemory()
    {
        using var searcher = new ManagementObjectSearcher(TotalAndFreeMemoryWmiQuery);
        foreach (var obj in searcher.Get())
        {
            var totalMemory = (ulong)obj[TotalMemoryProperty];
            var freeMemory = (ulong)obj[FreeMemoryProperty];
            return totalMemory - freeMemory;
        }

        throw new ApplicationException($"{TotalAndFreeMemoryWmiQuery} WMI Query not returned result!");
    }
}