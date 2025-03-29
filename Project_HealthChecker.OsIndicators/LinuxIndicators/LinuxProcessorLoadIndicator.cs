using Project_HealthChecker.Helpers.ClassExtensions;
using Project_HealthChecker.OsIndicators.BaseClasses;

namespace Project_HealthChecker.OsIndicators.LinuxIndicators;

public class LinuxProcessorLoadIndicator : BaseProcessorLoadIndicator
{
    public const string PathToStatFile = "/proc/stat";

    private ProcStat[] _previousStat = [];
    
    private ProcStat[] _actualStat = [];
    
    private Task? _indicationTask;

    private CancellationTokenSource? _indicationTaskCancellationTokenSource;

    public override float[] CoresLoad { get; protected set; } = [];

    public LinuxProcessorLoadIndicator()
        : base() { }

    public LinuxProcessorLoadIndicator(TimeSpan measurementInterval) 
        : base(measurementInterval)
    {
        int coresCount = Environment.ProcessorCount;
        CoresLoad = new float[coresCount];
    }
    
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
                _previousStat = _actualStat;
                _actualStat = await GetActualProcStatAsync();
                CoresLoad = CalculateAverageCpuCoresLoad(_previousStat, _actualStat);
                
                try
                {
                    await Task.Delay(base.MeasurementInterval, token);
                }
                catch (Exception)
                {
                    break;
                }
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

    private async Task<ProcStat[]> GetActualProcStatAsync()
    {
        var procStats = new List<ProcStat>();
        using var streamReader = File.OpenText(PathToStatFile);
        
        while (!streamReader.EndOfStream)
        {
            var currentString = (await streamReader.ReadLineAsync())!;
            
            if (!currentString.StartsWith("cpu"))
                break;
            
            procStats.Add(ProcStat.FromString(currentString));
        }

        return procStats[1..].ToArray();
    }

    private float[] CalculateAverageCpuCoresLoad(ProcStat[] previous, ProcStat[] actual)
    {
        var result = new float[actual.Length];
        actual.Foreach((element, number) =>
        {
            result[number] = CalculateAverageCpuCoreLoad(previous.ElementAtOrDefault(number), element);
        });
        
        return result;
    }

    private float CalculateAverageCpuCoreLoad(ProcStat previous, ProcStat actual)
    {
        var idleDifference = actual.Idle - previous.Idle;
        var iowaitDifference = actual.Iowait - previous.Iowait;
        var userDifference = actual.User - previous.User;
        var niceDifference = actual.Nice - previous.Nice;
        var systemDifference = actual.System - previous.System;
        var irqDifference = actual.Irq - previous.Irq;
        var softirqDifference = actual.Softirq - previous.Softirq;
        var stealDifference = actual.Steal - previous.Steal;

        var usefullTime = userDifference + niceDifference + systemDifference 
                          + irqDifference + softirqDifference + stealDifference;
        var downtime = idleDifference + iowaitDifference;
        var totalTime = usefullTime + downtime;

        return 100f * usefullTime / totalTime;
    }
}