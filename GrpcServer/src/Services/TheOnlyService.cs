using Grpc.Core;
using TheOnlyGen;
using Google.Protobuf;

namespace GrpcServer.Services;

public class TheOnlyService : TheOnly.TheOnlyBase
{
    const int maxBytesPerChunk = 24;
    byte[] buffer = new byte[maxBytesPerChunk];
    

    public TheOnlyService()
    {
        
    }

    private FileStream GetMatrixFileById(int id) {
        return File.OpenRead("./matrix.bin");
    }

    public override async Task GetInitialMatrix(
        GetInitialMatrixRequest request, 
        IServerStreamWriter<MatrixItems> responseStream,
        ServerCallContext context)
    {
        var matrixFile = GetMatrixFileById(request.MatrixId);
        int read = await matrixFile.ReadAsync(buffer);
        while (read != 0)
        {
            var chunkToTransfer = ByteString.CopyFrom(buffer, 0, read);
            var response = new MatrixItems { Items = chunkToTransfer };
            await responseStream.WriteAsync(response);
            read = await matrixFile.ReadAsync(buffer);
        }
    }

    public override Task<GetTaskResponse> GetTask(
        GetTaskRequest request,
        ServerCallContext context)
    {
        var response = new GetTaskResponse{ Column = 0 };
        response.PolynomParts.Add(new PolynomPart {
            Power = 2,
            Coefficient = 2,
        });
      
        return Task.FromResult(response);
    }

    public override async Task<Empty> SubmitResult(
        IAsyncStreamReader<ResultPart> requestStream,
        ServerCallContext context)
    {
        FileStream file = File.OpenWrite(Path.GetTempFileName());
        long taskId = 0;
        await foreach (var request in requestStream.ReadAllAsync())
        {
            taskId = request.TaskId;
            var buffer = new byte[request.Data.Length];
            request.Data.CopyTo(buffer, 0);
            await file.WriteAsync(buffer, 0, buffer.Length);
        }
        file.Close();
        File.Move(file.Name, $"task{taskId}_result.bin", overwrite: true);
        return new Empty {};
    }
}
