using System.Xml;
using MatrixFile;
using MatrixFile.Bytes;
using static System.Linq.Enumerable;
using static Utils;

public class ColumnPowerCalculator {
    IDictionary<int, FileStream> alreadyCalculatedPowers = new Dictionary<int, FileStream>();

    private Matrix initialMatrix;
    private int column;


    public ColumnPowerCalculator(Matrix initialMatrix, int column)
    {
        this.initialMatrix = initialMatrix;
        this.column = column;
    }


    public Matrix CalcPower(int neededPower)
    {
        var initialMetadata = initialMatrix.Metadata;
        var bufferMetadata = new Metadata
        {
            Rows = initialMetadata.Rows,
            Columns = 1,
        };

        var bufferFilePaths = new string[] {Path.GetTempFileName(), Path.GetTempFileName()};
        var bufferColumns = bufferFilePaths.Select(path =>
            {
                var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                bufferMetadata.WriteTo(file);
                file.Seek(0, SeekOrigin.Begin);
                return new ColumnStream(file, 0);
            }
        ).ToArray();
        using (var initialColumn = initialMatrix.GetColumn(column))
        {
            initialColumn.CopyTo(bufferColumns[0]);
        }
        foreach (var power in Range(1, neededPower-1))
        {
            var initialData = initialMatrix.GetData();
            foreach (var i in Range(0, initialMetadata.Rows))
            {
                int sum = 0;
                bufferColumns[0].SeekItem(0, SeekOrigin.Begin);
                foreach (var j in Range(0, initialMetadata.Columns))
                {
                    int a = bufferColumns[0].ReadItem();
                    int b = initialData.ReadItem();
                    sum += a*b;
                }
                bufferColumns[1].WriteItem(sum);
            }
            Swap(ref bufferColumns[0], ref bufferColumns[1]);
            Swap(ref bufferFilePaths[0], ref bufferFilePaths[1]);
        }
        alreadyCalculatedPowers.Add(neededPower, File.OpenRead(bufferFilePaths[0]));
        foreach (var buffer in bufferColumns)
        {
            buffer.Dispose();
        }
        File.Delete(bufferFilePaths[1]);
        return new Matrix(bufferFilePaths[0], FileAccess.Read);
    }
}