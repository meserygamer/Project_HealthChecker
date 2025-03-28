using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Project_HealthChecker.Helpers.ClassExtensions;
using Project_HealthChecker.OsIndicators.IndicatorInterfaces;

namespace Project_HealthChecker.OsIndicators.WindowsIndicators;

[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
public class WindowsProcessorLoadIndicator : IProcessorLoadIndicator
{
    public static readonly TimeSpan MinMeasurementInterval = TimeSpan.FromMilliseconds(200);
    
    private PerformanceCounter[] _processorCorePerformanceIndicators = null!;

    private readonly TimeSpan _measurementInterval;
    
    private Task? _indicationTask;

    private CancellationTokenSource? _indicationTaskCancellationTokenSource;
    
    public float[] CoresLoad { get; private set; }

    public WindowsProcessorLoadIndicator(TimeSpan measurementInterval)
    {
        _measurementInterval = MinMeasurementInterval <= measurementInterval
            ? measurementInterval
            : throw new ArgumentException($"{nameof(measurementInterval)} must be bigger than " +
                                          $"{nameof(MinMeasurementInterval)} or equal");
        
        SetPerformanceCounters();
    }
    
    public void Start()
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
                    await Task.Delay(_measurementInterval, token);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }, token);
    }

    public void Pause()
    {
        if (_indicationTaskCancellationTokenSource is null)
            return;
        
        _indicationTaskCancellationTokenSource.Cancel();
        _indicationTaskCancellationTokenSource = null;
        _indicationTask = null;
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