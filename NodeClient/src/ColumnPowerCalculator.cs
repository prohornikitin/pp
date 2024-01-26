using ComputingNodeGen;
using MatrixFile;
using static System.Linq.Enumerable;
using static NodeClient.Utils;

namespace NodeClient;
public class ColumnPolynomCalculator : IDisposable{
    private Matrix lastCalcedColumn;
    private IEnumerable<PolynomPart> polynomParts;
    private Matrix initialMatrix;
    private int column;
    private int lastCalcedPower = 1;

    public ColumnPolynomCalculator(Matrix initialMatrix, int column, IEnumerable<PolynomPart> polynomParts)
    {
        this.initialMatrix = initialMatrix;
        this.column = column;
        this.polynomParts = polynomParts;

        var bufferMetadata = new Metadata
        {
            Rows = initialMatrix.Rows,
            Columns = 1,
        };

        lastCalcedColumn = new Matrix(Path.GetTempFileName(), bufferMetadata, FileAccess.ReadWrite);

        using (ItemsStream initialColumn = initialMatrix.GetColumn(column), lastCalcedStream = lastCalcedColumn.GetColumn(0))
        {
            initialColumn.CopyTo(lastCalcedStream);
        }
    }

    
    public Matrix CalcPolynom(string resultPath)
    {
        var resultMetadata = initialMatrix.Metadata with {Columns = 1};
        var result = new Matrix(resultPath, resultMetadata, FileAccess.ReadWrite);
        foreach (var polynomPart in polynomParts)
        {
            var power = polynomPart.Power;
            var matrix = CalcPower(power) * polynomPart.Coefficient;
            result.Add(matrix);
        }
        return result;
    }
    
    private Matrix CalcPower(int power)
    {
        if (power < lastCalcedPower)
        {
            throw new ArgumentException("power cannot be less than lastCalcedPower (maybe polynomParts aren't sored?)");
        }

        for (int i = lastCalcedPower; i < power; i++)
        {
            CalcNextPower();
        }

        return lastCalcedColumn;
    }

    private Matrix CalcNextPower()
    {
        var tempMatrix = new Matrix(Path.GetTempFileName(), lastCalcedColumn.Metadata, FileAccess.ReadWrite);
        using(ItemsStream initialData = initialMatrix.GetData(), lastCalcedStream = lastCalcedColumn.GetColumn(0), tempStream = tempMatrix.GetColumn(0)) {
            foreach (var i in Range(0, initialMatrix.Rows))
            {
                int sum = 0;
                lastCalcedStream.SeekItem(0, SeekOrigin.Begin);
                foreach (var j in Range(0, initialMatrix.Columns))
                {
                    int a = lastCalcedStream.ReadItem();
                    int b = initialData.ReadItem();
                    sum += a*b;
                }
                tempStream.WriteItem(sum);
            }
        }
        Swap(ref tempMatrix, ref lastCalcedColumn);
        File.Delete(tempMatrix.FilePath);

        lastCalcedPower++;
        return lastCalcedColumn;
    }

    public void Dispose()
    {  
        File.Delete(lastCalcedColumn.FilePath);
    }
}