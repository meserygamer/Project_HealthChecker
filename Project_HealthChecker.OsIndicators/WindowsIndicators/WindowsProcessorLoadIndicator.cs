using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Project_HealthChecker.OsIndicators.BaseClasses;

namespace Project_HealthChecker.OsIndicators.WindowsIndicators;

[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
public class WindowsProcessorLoadIndicator : BaseProcessorLoadIndicator
{
    private PerformanceCounter[] _processorCorePerformanceIndicators = null!;
    
    private Task? _indicationTask;

    private CancellationTokenSource? _indicationTaskCancellationTokenSource;

    public override float[] CoresLoad { get; protected set; } = [];

    public WindowsProcessorLoadIndicator()
        : base()
    {
        SetPerformanceCounters();
    }

    public WindowsProcessorLoadIndicator(TimeSpan measurementInterval) 
        : base(measurementInterval)
    {
        SetPerformanceCounters();
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
                CoresLoad = _processorCorePerformanceIndicators
                    .Select(coreIndicator => coreIndicator.NextValue())
                    .ToArray();
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

    private void SetPerformanceCounters()
    {
        int coresCount = Environment.ProcessorCount;
        CoresLoad = new float[coresCount];
        _processorCorePerformanceIndicators = Enumerable
            .Range(0, coresCount)
            .Select(coreNumber => 
                new PerformanceCounter("Processor", "% Processor Time", coreNumber.ToString()))
            .ToArray();
    }
}