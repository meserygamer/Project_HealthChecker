namespace Project_HealthChecker.OsIndicators.IndicatorInterfaces;

public interface IProcessorLoadIndicator
{
    float[] CoresLoad { get; }

    void Start();
    
    void Pause();
}