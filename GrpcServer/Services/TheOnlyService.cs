using Grpc.Core;
using TheOnlyGen;
using MatrixFile.Bytes;
using System.Text;

namespace GrpcServer.Services;

public class TheOnlyService : TheOnly.TheOnlyBase
{
    private readonly FileStream matrix;

    public TheOnlyService()
    {
        var path = "./matrix.bin";
        matrix = File.OpenRead(path);
    }

    public override async Task GetInitialMatrix(
        GetInitialMatrixRequest request, 
        IServerStreamWriter<GetInitialMatrixReply> responseStream,
        ServerCallContext context)
    {
        using (var items = new ItemsStream(matrix))
        {
            using (var reader = new BinaryReader(items, Encoding.UTF8, false))
            {
                try 
                {
                    foreach (var i in Enumerable.Range(1, (int)items.Length))
                    {
                        var response = new GetInitialMatrixReply();
                        response.Items.Add(reader.ReadInt32());
                        await responseStream.WriteAsync(response);
                    }
                }
                catch (EndOfStreamException)
                {
                    // Ignore
                }
            }
        }
    }

    public override async Task GetInitialMatrixRow(
        GetInitialMatrixRowRequest request, 
        IServerStreamWriter<GetInitialMatrixReply> responseStream,
        ServerCallContext context)
    {
        using (var row = new RowStream(matrix, request.Row))
        {
            using (var reader = new BinaryReader(row, Encoding.UTF8, false))
            {
                try 
                {
                    foreach (var i in Enumerable.Range(1, (int)row.Length))
                    {
                        var response = new GetInitialMatrixReply();
                        response.Items.Add(reader.ReadInt32());
                        await responseStream.WriteAsync(response);
                    }
                }
                catch (EndOfStreamException)
                {
                    // Ignore
                }
            }
        }
    }

    public override async Task GetInitialMatrixColumn(
        GetInitialMatrixColumnRequest request, 
        IServerStreamWriter<GetInitialMatrixReply> responseStream,
        ServerCallContext context)
    {
        using (var items = new ColumnStream(matrix, request.Column))
        {
            using (var reader = new BinaryReader(items, Encoding.UTF8, false))
            {
                try 
                {
                    foreach (var i in Enumerable.Range(1, (int)items.Length))
                    {
                        var response = new GetInitialMatrixReply();
                        response.Items.Add(reader.ReadInt32());
                        await responseStream.WriteAsync(response);
                    }
                }
                catch (EndOfStreamException)
                {
                    // Ignore
                }
            }
        }
    }

    public override Task<GetTaskReply> GetTask(
        GetTaskRequest request,
        ServerCallContext context)
    {
        return Task.FromResult(new GetTaskReply{
            Row = 0
        });
    }
}
