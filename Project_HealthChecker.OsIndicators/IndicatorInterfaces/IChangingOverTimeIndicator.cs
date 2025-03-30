namespace Project_HealthChecker.OsIndicators.IndicatorInterfaces;

public interface IChangingOverTimeIndicator
{
    TimeSpan MeasurementInterval { get; set; }

    void Start();
    
    void Pause();
}