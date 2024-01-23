using Grpc.Core;
using ComputingNodeGen;
using Google.Protobuf;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Services;

public class ComputingNodeService : ComputingNode.ComputingNodeBase
{
    const int maxBytesPerChunk = 24;
    byte[] buffer = new byte[maxBytesPerChunk];
    private readonly TheOnlyDbContext db;
    public ComputingNodeService(
        TheOnlyDbContext dbContext
    )
    {
        this.db = dbContext;
    }

    private async Task<FileStream?> GetMatrixFileById(long id) {
        var matrix = await db.Matrices.FindAsync(id);
        if (matrix == null)
        {
            return null;
        }
        return File.OpenRead(matrix.FilePath);
    }

    public override async Task GetInitialMatrix(
        GetInitialMatrixRequest request, 
        IServerStreamWriter<MatrixItems> responseStream,
        ServerCallContext context)
    {
        var matrixFile = await GetMatrixFileById(request.MatrixId);
        if(matrixFile == null)
        {
            context.Status = new Status(StatusCode.NotFound, "matrix not found");
            return;
        }

        int read = await matrixFile.ReadAsync(buffer);
        while (read != 0)
        {
            var chunkToTransfer = ByteString.CopyFrom(buffer, 0, read);
            var response = new MatrixItems { Items = chunkToTransfer };
            await responseStream.WriteAsync(response);
            read = await matrixFile.ReadAsync(buffer);
        }
    }

    public override async Task<GetTaskResponse> GetTask(
        GetTaskRequest request,
        ServerCallContext context)
    {
        var userTask = await db.UserTasks.FirstOrDefaultAsync(
            t => t.UnscheduledColumns.Start != t.UnscheduledColumns.End
        );
        if (userTask == null)
        {
            context.Status = new Status(StatusCode.NotFound, "task not found");
            return new GetTaskResponse() {};
        }
        var column = userTask.UnscheduledColumns.End-1;
        userTask.UnscheduledColumns.End--;
        await db.SaveChangesAsync();
        var nodeTask = new NodeTask {
            UserTask = userTask,
            column = column,
        };
        await db.AddAsync(nodeTask);
        await db.SaveChangesAsync();
        var response = new GetTaskResponse{
            InitialMatrixId = userTask.InitialMatrixId,
            Column = nodeTask.column,
            TaskId = nodeTask.Id,
        };
        response.PolynomParts.AddRange(userTask.Polynom.Select(
            (p)=> new ComputingNodeGen.PolynomPart {
                Coefficient = p.Coefficient,
                Power = p.Power,
            }
        ));
        return response;
    }


    public override async Task<Empty> SubmitResult(
        IAsyncStreamReader<ResultPart> requestStream,
        ServerCallContext context)
    {
        var temporaryPath = Path.GetTempFileName();
        long nodeTaskId = 0;
        using(FileStream file = File.OpenWrite(temporaryPath))
        {
            await foreach (var request in requestStream.ReadAllAsync())
            {
                nodeTaskId = request.TaskId;
                var buffer = new byte[request.Data.Length];
                request.Data.CopyTo(buffer, 0);
                await file.WriteAsync(buffer, 0, buffer.Length);
            }
        }
        var nodeTask = await db.NodeTasks.FindAsync(nodeTaskId);
        if (nodeTask == null)
        {
            context.Status = new Status(StatusCode.NotFound, "task not found");
            return new Empty{};
        }
        
        var resultPath = $"matrices/results/task_{nodeTaskId}.bin";
        File.Move(temporaryPath, resultPath, overwrite: true);
        var resultMatrix = Matrix.WithExistingFile(resultPath);
        await db.Matrices.AddAsync(resultMatrix);
        nodeTask.result = resultMatrix;
        db.NodeTasks.Update(nodeTask);
        await db.SaveChangesAsync();
        return new Empty {};
    }
}
