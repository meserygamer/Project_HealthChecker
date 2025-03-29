namespace Project_HealthChecker.OsIndicators.IndicatorInterfaces;

public interface IProcessorLoadIndicator
{
    float[] CoresLoad { get; }

    TimeSpan MeasurementInterval { get; set; }

    void Start();
    
    void Pause();
}