// using MatrixFile.Bytes;
// using System.Text;

// var path = "./matrix.bin";
// var file = File.OpenRead(path);

// var stream = new ItemsStream(file);
// var buff = new byte[8];
// stream.Read(buff);
// Console.WriteLine(string.Join(", ", buff));


// using (var items = new ItemsStream(file))
// {
//     using (var reader = new BinaryReader(items, Encoding.UTF8, false))
//     {
//         try
//         {
//             foreach (var i in Enumerable.Range(1, (int)items.Length)) {
//                 Console.WriteLine(reader.ReadInt32());
//             }
//         }
//         catch (EndOfStreamException)
//         {
//             // Ignore
//         }
//     }
// }

using System.Threading.Tasks;
using Grpc.Net.Client;
using TheOnlyGen;
using GrpcProtoGen;

// The port number must match the port of the gRPC server.
var channel = GrpcChannel.ForAddress("http://localhost:5113");
var client = new TheOnly.TheOnlyClient(channel);
var response = client.GetInitialMatrix(new GetInitialMatrixRequest());

CancellationTokenSource source = new CancellationTokenSource();
CancellationToken cancellationToken = source.Token;
while(await response.ResponseStream.MoveNext(cancellationToken))
{
    Console.WriteLine("Into while loop");
    var current = response.ResponseStream.Current;
    Console.WriteLine($"{current}");
}
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
