﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Project_HealthChecker.OsIndicators.BaseClasses;
using Project_HealthChecker.OsIndicators.IndicatorInterfaces;

namespace Project_HealthChecker.OsIndicators.WindowsIndicators;

[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
public class WindowsProcessorLoadIndicator : BaseChangingOverTimeIndicator, IProcessorLoadIndicator
{
    private PerformanceCounter[] _processorCorePerformanceIndicators = null!;
    
    private Task? _indicationTask;

    private CancellationTokenSource? _indicationTaskCancellationTokenSource;

    public float[] CoresLoad { get; private set; } = [];

    public WindowsProcessorLoadIndicator()
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