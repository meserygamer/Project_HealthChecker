using Project_HealthChecker.Helpers.ClassExtensions;
using Project_HealthChecker.OsIndicators.BaseClasses;
using Project_HealthChecker.OsIndicators.IndicatorInterfaces;

namespace Project_HealthChecker.OsIndicators.LinuxIndicators;

public class LinuxRamUsingIndicator : BaseChangingOverTimeIndicator, IRamUsingIndicator
{
    private const string PathToMemInfoFile = "/proc/meminfo";
    
    private const int BytesInKilobyte = 1024;
    
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
                using var memInfoStream = File.OpenText(PathToMemInfoFile);
                ulong ramTotal = GetRamInfoFromFileLine((await memInfoStream.ReadLineAsync(token))!);
                ulong ramFree = GetRamInfoFromFileLine((await memInfoStream.ReadLineAsync(token))!);
                UsingMemory = (ramTotal - ramFree) * BytesInKilobyte;
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
    
    protected override void OnMeasurementIntervalChanged()
    {
        base.OnMeasurementIntervalChanged();
        
        if (_indicationTaskCancellationTokenSource is null)
            return;

        Pause();
        Start();
    }

    private ulong GetRamInfoFromFileLine(string fileLine)
    {
        if (string.IsNullOrEmpty(fileLine))
            throw new ArgumentException($"Аргумент {nameof(fileLine)} пустой или null!");
        
        return Convert.ToUInt64(
            fileLine
                .CollapseSpaces()
                .Split(" ")[1]);
    }
}