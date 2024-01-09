using Grpc.Core;
using TheOnlyGen;

namespace GrpcServer.Services;

public class TheOnlyService : TheOnly.TheOnlyBase
{
    // private readonly ILogger<TheOnlyService> _logger;
    // public TheOnlyService(ILogger<TheOnlyService> logger)
    // {
    //     _logger = logger;
    // }

    public override async Task GetInitialMatrix(
        GetInitialMatrixRequest request, 
        IServerStreamWriter<GetInitialMatrixReply> responseStream,
        ServerCallContext context)
    {
        var response = new GetInitialMatrixReply();
        response.Items.Add(-1);
        await responseStream.WriteAsync(response);
    }
}
