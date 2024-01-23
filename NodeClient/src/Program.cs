using Google.Protobuf;
using Grpc.Net.Client;
using MatrixFile;
using ComputingNodeGen;
using Grpc.Core;

internal class Program
{
    private static async Task Main()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5113");
        var client = new ComputingNode.ComputingNodeClient(channel);
        for(int i = 0; i < 100000; ++i) 
        {
            GetTaskResponse? task = null;
            try
            {
                task = await client.GetTaskAsync(new GetTaskRequest());
            }
            catch(RpcException e)
            {
                if(e.Status.StatusCode != StatusCode.NotFound)
                {
                    throw;
                }
                Console.WriteLine("task not found");
                await Task.Delay(5000);
            }
            if(task == null)
            {
                continue;
            }
            var matrix = await GetInitialMatrix(client, task.InitialMatrixId);
            var result = await Task.Run(()=>ProcessTask(task, matrix));
            await SubmitResult(client, result, task.TaskId);
            Console.WriteLine("Ok");
        }
    }

    private static async Task<Matrix> GetInitialMatrix(ComputingNode.ComputingNodeClient client, long matrixId)
    {
        using (var initialMatrixFile = File.OpenWrite("initial.bin"))
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
        return new Matrix("initial.bin");
    }

    private static async Task SubmitResult(ComputingNode.ComputingNodeClient client, Matrix result, long taskId)
    {
        using var call = client.SubmitResult();
        const int sizeOfBuffer = 24;
        var buffer = new byte[sizeOfBuffer];
        using (var data = result.OpenFile()) {
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
        var calculator = new ColumnPowerCalculator(initialMatrix, task.Column);
        var resultMetadata = initialMatrix.Metadata with {Columns = 1};
        var result = new Matrix("output.bin", resultMetadata, FileAccess.ReadWrite);
        foreach (var polynomPart in task.PolynomParts)
        {
            var power = polynomPart.Power;
            var matrix = calculator.CalcPower(power) * polynomPart.Coefficient;
            result.Add(matrix);
        }
        return result;
    }


}