using Project_HealthChecker.OsIndicators.IndicatorInterfaces;

namespace Project_HealthChecker.OsIndicators.BaseClasses;

public abstract class BaseProcessorLoadIndicator : IProcessorLoadIndicator
{
    public static readonly TimeSpan MinMeasurementInterval = TimeSpan.FromMilliseconds(200);

    private TimeSpan _measurmentInterval;

    protected BaseProcessorLoadIndicator() { }
    
    protected BaseProcessorLoadIndicator(TimeSpan measurementInterval)
    {
        MeasurementInterval = measurementInterval;
    }

    public TimeSpan MeasurementInterval
    {
        get => _measurmentInterval;
        set
        {
            if (MinMeasurementInterval > value)
                throw new ArgumentException($"{nameof(value)} must be bigger than " +
                                            $"{nameof(MinMeasurementInterval)} or equal");

            _measurmentInterval = value;
            OnMeasurementIntervalChanged();
        }
    }

    public abstract float[] CoresLoad { get; protected set; }

    protected virtual void OnMeasurementIntervalChanged() { }

    public abstract void Start();

    public abstract void Pause();
}