using Grpc.Core;
using ComputingNodeGen;
using Google.Protobuf;
using Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;

namespace Server.GrpcServices;

public class ComputingNodeService : ComputingNode.ComputingNodeBase
{
    const int maxBytesPerChunk = 24;
    byte[] buffer = new byte[maxBytesPerChunk];
    private readonly TheOnlyDbContext db;
    private readonly string resultsDir;
    public ComputingNodeService(
        TheOnlyDbContext dbContext,
        IConfiguration configuration
    )
    {
        db = dbContext;
        resultsDir = configuration["Custom:MatricesDirectory"]!;
        Directory.CreateDirectory(resultsDir);
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

    private async Task<NodeTask?> TryScheduleNextTask(IEnumerable<long> preferredMatrixIds)
    {
        var previouslyFailedNodeTask = await db.NodeTasks.Include(t => t.UserTask).FirstOrDefaultAsync(
            t => t.State == TaskState.Fail
        );
        if (previouslyFailedNodeTask != null)
        {
            previouslyFailedNodeTask.State = TaskState.WorkInProgress;
            return previouslyFailedNodeTask;
        }
        
        var userTask = await db.UserTasks
            .Include(t => t.InitialMatrix)
            .FirstOrDefaultAsync(t => 
                    (t.UnscheduledColumns.Start != t.UnscheduledColumns.End) && 
                    preferredMatrixIds.Contains(t.InitialMatrixId)
            );
        userTask ??= await db.UserTasks
            .Include(t => t.InitialMatrix)
            .FirstOrDefaultAsync(
                //manual check for Empty range because linq doesn't support custom methods
                t => t.UnscheduledColumns.Start != t.UnscheduledColumns.End 
            );
        if (userTask == null)
        {
            return null;
        }
        
        var column = userTask.UnscheduledColumns.End-1;
        userTask.UnscheduledColumns.End--;
        await db.SaveChangesAsync();
        var nodeTask = new NodeTask {
            UserTask = userTask,
            column = column,
            UserTaskId = userTask.Id,
        };
        await db.AddAsync(nodeTask);
        await db.SaveChangesAsync();
        return nodeTask;
    }

    public override async Task<GetTaskResponse> GetTask(
        GetTaskRequest request,
        ServerCallContext context)
    {
        var task = await TryScheduleNextTask(request.PreferredMatrixIds);
        while (task == null)
        {
            await db.WaitForTaskAdd(context.CancellationToken);
            task = await TryScheduleNextTask(request.PreferredMatrixIds);
        }
        var response = new GetTaskResponse{
            InitialMatrixId = task.UserTask.InitialMatrixId,
            Column = task.column,
            TaskId = task.Id,
        };
        response.PolynomParts.AddRange(task.UserTask.Polynom.Select(
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
        await requestStream.MoveNext();
        long nodeTaskId = requestStream.Current.TaskId;
        var nodeTask = await db.NodeTasks
            .Include(t => t.UserTask)
            .ThenInclude(u => u.Result)
            .SingleOrDefaultAsync(t => t.Id == nodeTaskId);
        if (nodeTask == null)
        {
            context.Status = new Status(StatusCode.NotFound, "task not found");
            return new Empty{};
        }
        var matrix = new MatrixFile.Matrix(nodeTask!.UserTask.Result.FilePath, FileAccess.ReadWrite);
        using(Stream columnData = matrix.GetColumn(nodeTask.column))
        {
            do
            {
                var request = requestStream.Current;
                nodeTaskId = request.TaskId;
                var buffer = new byte[request.Data.Length];
                request.Data.CopyTo(buffer, 0);
                await columnData.WriteAsync(buffer, 0, buffer.Length);
            } 
            while(await requestStream.MoveNext());
        }
        db.NodeTasks.Remove(nodeTask);
        await db.SaveChangesAsync();
        if (!nodeTask.UserTask.UnscheduledColumns.IsEmpty())
        {
            return new Empty {};
        }
        var remainsWorking = await db.NodeTasks
            .Where(t => t.UserTaskId == nodeTask.UserTaskId)
            .CountAsync();
        if(remainsWorking > 0) {
            return new Empty {};
        }
        
        nodeTask.UserTask.State = TaskState.ResultReady;
        await db.SaveChangesAsync();
        return new Empty {};
    }

    public override async Task<Empty> ReportNodeError(
        ReportNodeErrorRequest request,
        ServerCallContext context)
    {
        var task = await db.NodeTasks.FindAsync(request.TaskId);
        if(task == null)
        {
            context.Status = new Status(StatusCode.NotFound, "task not found");
            return new Empty{};
        }
        task.State = TaskState.Fail;
        await db.SaveChangesAsync();
        return new Empty {};
    }
}