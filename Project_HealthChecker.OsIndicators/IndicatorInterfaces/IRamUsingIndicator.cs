namespace Project_HealthChecker.OsIndicators.IndicatorInterfaces;

public interface IRamUsingIndicator : IChangingOverTimeIndicator
{
    ulong UsingMemory { get; }
}