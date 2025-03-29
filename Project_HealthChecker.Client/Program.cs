using Grpc.Net.Client;
using Project_HealthChecker.Helpers.ClassExtensions;
using Project_HealthChecker.OsIndicators.WindowsIndicators;
using Project_HealthChecker.ProtoContracts;

namespace Project_HealthChecker.Client;

class Program
{
    static async Task Main(string[] args)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        using var channel = GrpcChannel.ForAddress("http://localhost:44000");
        var client = new CpuInfo.CpuInfoClient(channel);
        
        var serverData = client.GetCoreLoad(new EmptyRequest());
        var responseStream = serverData.ResponseStream;
        
        while (await responseStream.MoveNext(new CancellationToken()))
        {
            CoreLoadResponse response = responseStream.Current;
            Console.Clear();
            response.CoresLoad.Foreach((coreLoad, coreNumber) =>
            {
                Console.WriteLine($"Core №{coreNumber} - {coreLoad}%");
            });
            Console.WriteLine();
        }
    }
}