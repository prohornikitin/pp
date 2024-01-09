var path = "./matrix.bin";
var file = File.OpenRead(path);



void WriteMatrixRow(FileStream destination, int[] numbers)
{
    var bytes = numbers.SelectMany(BitConverter.GetBytes).ToArray();
    Console.WriteLine(string.Join(", ", bytes));
    destination.Write(bytes);
}


var stream = new MatrixFileColumnStream(file, 0);
var buff = new byte[8];
stream.Read(buff);
Console.WriteLine(string.Join(", ", buff));