using Grpc.Core;
using TheOnlyGen;
using MatrixFile.Bytes;
using Google.Protobuf;

namespace GrpcServer.Services;

public class TheOnlyService : TheOnly.TheOnlyBase
{
    private readonly FileStream matrix;
    private readonly MatrixFile.Metadata matrixMetadata;
    const int maxBytesPerChunk = 24;
    byte[] buffer = new byte[maxBytesPerChunk];
    

    public TheOnlyService()
    {
        var path = "./matrix.bin";
        matrix = File.OpenRead(path);
        matrixMetadata = MatrixFile.Metadata.ReadFrom(matrix);
    }

    public override async Task GetInitialMatrix(
        Empty request, 
        IServerStreamWriter<MatrixItems> responseStream,
        ServerCallContext context)
    {
        var items = new ItemsStream(matrix);
        int read = await items.ReadAsync(buffer);
        while (read != 0)
        {
            var chunkToTransfer = ByteString.CopyFrom(buffer, 0, read);
            var response = new MatrixItems { Items = chunkToTransfer };
            await responseStream.WriteAsync(response);
            read = await items.ReadAsync(buffer);
        }
    }

    public override async Task GetInitialMatrixRow(
        GetInitialMatrixRowRequest request, 
        IServerStreamWriter<MatrixItems> responseStream,
        ServerCallContext context)
    {
        if(request.Row < 0 || request.Row >= matrixMetadata.Rows) {
            context.Status = new Status(StatusCode.InvalidArgument, "Invalid Row");
        }
        var items = new RowStream(matrix, request.Row);
        int read = await items.ReadAsync(buffer);
        while (read != 0)
        {
            var chunkToTransfer = ByteString.CopyFrom(buffer, 0, read);
            var response = new MatrixItems { Items = chunkToTransfer };
            await responseStream.WriteAsync(response);
            read = await items.ReadAsync(buffer);
        }
    }

    public override async Task GetInitialMatrixColumn(
        GetInitialMatrixColumnRequest request, 
        IServerStreamWriter<MatrixItems> responseStream,
        ServerCallContext context)
    {
        if(request.Column < 0 || request.Column >= matrixMetadata.Columns) {
            context.Status = new Status(StatusCode.InvalidArgument, "Invalid Column");
        }

        var items = new ColumnStream(matrix, request.Column);
        int read = await items.ReadAsync(buffer);
        while (read != 0)
        {
            var chunkToTransfer = Google.Protobuf.ByteString.CopyFrom(buffer, 0, read);
            var response = new MatrixItems { Items = chunkToTransfer };
            await responseStream.WriteAsync(response);
            read = await items.ReadAsync(buffer);
        }
    }

    public override Task<GetTaskReply> GetTask(
        Empty request,
        ServerCallContext context)
    {
        var response = new GetTaskReply{ Row = 0 };
        response.PolynomParts.Add(new GetTaskReply.Types.PolynomPart {
            Power = 2,
            Coefficient = ByteString.CopyFrom(BitConverter.GetBytes(2)),
        });
        return Task.FromResult(response);
    }

    public override Task<MatrixMetadata> GetInitialMatrixMetadata(
        Empty request,
        ServerCallContext context)
    {
        return Task.FromResult(new MatrixMetadata {
            Rows = matrixMetadata.Rows,
            Columns = matrixMetadata.Columns,
            ItemDataType = (ItemDataType)(int)matrixMetadata.itemType
        });
    }
}
