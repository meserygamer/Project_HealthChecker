using Grpc.Core;
using Project_HealthChecker.ProtoContracts;

namespace Project_HealthChecker.Server.Services;

public class RamInfoService : RamInfo.RamInfoBase
{
    public override Task<TotalMemoryResponse> GetTotalMemory(EmptyRequest request, ServerCallContext context)
    {
        return base.GetTotalMemory(request, context);
    }
}