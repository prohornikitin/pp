using Grpc.Net.Client;
using MatrixFile;
using MatrixFile.Bytes;
using TheOnlyGen;
using static System.Linq.Enumerable;

internal class Program
{
    private static Metadata initialMetadata;
    private static Metadata bufferMetadata;
    private static async Task Main(string[] args)
    {
        // The port number must match the port of the gRPC server.
        var channel = GrpcChannel.ForAddress("http://localhost:5113");
        var client = new TheOnly.TheOnlyClient(channel);
        var task = await client.GetTaskAsync(new Empty());


        var metadataResponse = await client.GetInitialMatrixMetadataAsync(new Empty { });
        initialMetadata = new Metadata
        {
            Rows = metadataResponse.Rows,
            Columns = metadataResponse.Columns,
        };
        bufferMetadata = new Metadata
        {
            Rows = initialMetadata.Rows,
            Columns = 1,
        };

        using (var initialMatrixFile = File.OpenWrite("initial.bin"))
        {
            initialMetadata.WriteTo(initialMatrixFile);

            var response = client.GetInitialMatrix(new Empty { });
            var cancellationToken = new CancellationToken();
            while (await response.ResponseStream.MoveNext(cancellationToken))
            {
                var chunk = response.ResponseStream.Current.Items;
                var buffer = new byte[chunk.Length];
                chunk.CopyTo(buffer, 0);
                await initialMatrixFile.WriteAsync(buffer, cancellationToken);
            }
            initialMatrixFile.Close();
        }


        var polynomPartsIter = task.PolynomParts.GetEnumerator();
        var maxPower = (task.PolynomParts.MaxBy(x => x.Power) ?? throw new Exception("Empty task")).Power;
    }

    static async void CalcPower(int power, int row)
    {
        var initialFile = File.OpenRead("power1.bin");
        var bufferFile = File.OpenWrite("buff.bin");
        bufferMetadata.WriteTo(bufferFile);
        var bufferItems = new ItemsStream(bufferFile);
        //TODO: write one row to buff.bin;
        foreach (var p in Range(0, power))
        {
            foreach (var i in Range(0, initialMetadata.Columns))
            {

            }
        }
        
    }
}
// while(polynomPartsIter.MoveNext()) {

// }