using Project_HealthChecker.OsIndicators.IndicatorInterfaces;

namespace Project_HealthChecker.OsIndicators.BaseClasses;

public abstract class BaseChangingOverTimeIndicator : IChangingOverTimeIndicator
{
    private TimeSpan _measurementInterval;

    protected BaseChangingOverTimeIndicator() { }
    
    protected BaseChangingOverTimeIndicator(TimeSpan measurementInterval)
    {
        MeasurementInterval = measurementInterval;
    }

    public virtual TimeSpan MinMeasurementInterval { get; protected init; } = TimeSpan.FromMilliseconds(200);

    public TimeSpan MeasurementInterval
    {
        get => _measurementInterval;
        set
        {
            if (MinMeasurementInterval > value)
                throw new ArgumentException($"{nameof(value)} must be bigger than " +
                                            $"{nameof(MinMeasurementInterval)} or equal");

            _measurementInterval = value;
            OnMeasurementIntervalChanged();
        }
    }

    protected virtual void OnMeasurementIntervalChanged() { }

    public abstract void Start();

    public abstract void Pause();
}