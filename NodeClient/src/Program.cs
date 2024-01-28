using Google.Protobuf;
using Grpc.Net.Client;
using MatrixFile;
using ComputingNodeGen;
using ComputingNodeClient = ComputingNodeGen.ComputingNode.ComputingNodeClient;

namespace NodeClient;
internal class Program
{
    private static readonly string matricesDir = "matricesTemporary";
    private static async Task Main()
    {
        Directory.CreateDirectory(matricesDir);

        var channel = GrpcChannel.ForAddress("http://localhost:5113");
        var client = new ComputingNodeClient(channel);
        GetTaskResponse? task = null;
        try
        {
            while(true)
            {
                task = await client.GetTaskAsync(new GetTaskRequest());
                Console.WriteLine($"task {task.TaskId} found");

                var matrix = await GetInitialMatrix(client, task.InitialMatrixId);

                var result = await Task.Run(()=>ProcessTask(task, matrix));
                await SubmitResult(client, result, task.TaskId);
                Console.WriteLine("Ok");
            }
        } catch (Exception e) {
            if (task != null) {
                await SubmitError(client, task.TaskId, e.Message);
            }
            throw;
        }
        
    }

    private static async Task SubmitError(ComputingNodeClient client, long taskId, string errorDescription)
    {
        await client.ReportNodeErrorAsync(new ReportNodeErrorRequest {
            TaskId = taskId,
            Description = errorDescription,
        });
    }

    private static async Task<Matrix> GetInitialMatrix(ComputingNodeClient client, long matrixId)
    {
        var initialMatrixPath = Path.Join(matricesDir, $"{matrixId}.bin");
        if(File.Exists(initialMatrixPath)) {
            return new Matrix(initialMatrixPath);
        }
        using (var initialMatrixFile = File.OpenWrite(initialMatrixPath))
        {
            var response = client.GetInitialMatrix(new GetInitialMatrixRequest {
                MatrixId = matrixId
            });
            var cancellationToken = new CancellationToken();
            while (await response.ResponseStream.MoveNext(cancellationToken))
            {
                var chunk = response.ResponseStream.Current.Items;
                await Task.Run(() => chunk.WriteTo(initialMatrixFile));
            }
        }
        return new Matrix(initialMatrixPath);
    }

    private static async Task SubmitResult(ComputingNodeClient client, Matrix result, long taskId)
    {
        using var call = client.SubmitResult();
        const int sizeOfBuffer = 24;
        var buffer = new byte[sizeOfBuffer];
        using (var data = result.GetData()) {
            for (int i = 0; i < ((data.Length + sizeOfBuffer)/ sizeOfBuffer); ++i)
            {
                await data.ReadAsync(buffer);
                await call.RequestStream.WriteAsync(new ResultPart {
                    TaskId = taskId,
                    Data = ByteString.CopyFrom(buffer),
                });
            }
        }
        await call.RequestStream.CompleteAsync();
        var response = await call;
    }

    private static Matrix ProcessTask(GetTaskResponse task, Matrix initialMatrix)
    {
        var resultPath = Path.Join(matricesDir, "output.bin");
        var calculator = new ColumnPolynomCalculator(initialMatrix, task.Column, task.PolynomParts);
        return calculator.CalcPolynom(resultPath);
    }
}