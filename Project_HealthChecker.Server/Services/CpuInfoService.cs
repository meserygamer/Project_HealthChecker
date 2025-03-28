using System.Diagnostics;
using Grpc.Core;
using Project_HealthChecker.OsIndicators.WindowsIndicators;
using Project_HealthChecker.ProtoContracts;

namespace Project_HealthChecker.Server.Services;

public class CpuInfoService : CpuInfo.CpuInfoBase
{
    public override async Task GetCoreLoad(EmptyRequest request, IServerStreamWriter<CoreLoadResponse> responseStream, ServerCallContext context)
    {
        var windowsProcessorLoadIndicator = new WindowsProcessorLoadIndicator(TimeSpan.FromMilliseconds(500));
        windowsProcessorLoadIndicator.Start();

        try
        {
            while (true)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                await responseStream.WriteAsync(new CoreLoadResponse
                    { CoresLoad = { windowsProcessorLoadIndicator.CoresLoad } });
                await Task.Delay(TimeSpan.FromMilliseconds(500), context.CancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Trace.WriteLine("Клиент отключился!");
            Trace.Flush();
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);
            Trace.Flush();
        }
        finally
        {
            windowsProcessorLoadIndicator.Pause();
        }
    }
}