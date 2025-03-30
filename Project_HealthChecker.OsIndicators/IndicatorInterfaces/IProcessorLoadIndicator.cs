namespace Project_HealthChecker.OsIndicators.IndicatorInterfaces;

public interface IProcessorLoadIndicator : IChangingOverTimeIndicator
{
    float[] CoresLoad { get; }
}