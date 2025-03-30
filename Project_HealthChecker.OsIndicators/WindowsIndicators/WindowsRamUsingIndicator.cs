using Project_HealthChecker.OsIndicators.BaseClasses;
using Project_HealthChecker.OsIndicators.IndicatorInterfaces;

namespace Project_HealthChecker.OsIndicators.WindowsIndicators;

public class WindowsRamUsingIndicator : BaseChangingOverTimeIndicator, IRamUsingIndicator
{
    public ulong UsingMemory { get; }
    
    public override void Start()
    {
        throw new NotImplementedException();
    }

    public override void Pause()
    {
        throw new NotImplementedException();
    }
}