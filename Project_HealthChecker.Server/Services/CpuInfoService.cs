using Grpc.Core;
using Project_HealthChecker.OsIndicators.IndicatorInterfaces;
using Project_HealthChecker.ProtoContracts;

namespace Project_HealthChecker.Server.Services;

public class CpuInfoService : CpuInfo.CpuInfoBase
{
    private readonly IProcessorLoadIndicator _processorLoadIndicator;

    private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(500);
    
    public CpuInfoService(IProcessorLoadIndicator processorLoadIndicator)
    {
        _processorLoadIndicator = processorLoadIndicator;
    }
    
    public override async Task GetCoreLoad(EmptyRequest request, IServerStreamWriter<CoreLoadResponse> responseStream, ServerCallContext context)
    {
        _processorLoadIndicator.MeasurementInterval = _updateInterval;
        _processorLoadIndicator.Start();

        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await responseStream.WriteAsync(new CoreLoadResponse 
                    { CoresLoad = { _processorLoadIndicator.CoresLoad } });
                await Task.Delay(_updateInterval, context.CancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Клиент отключился!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            _processorLoadIndicator.Pause();
        }
    }
}