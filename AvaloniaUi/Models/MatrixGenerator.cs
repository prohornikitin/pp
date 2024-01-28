using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using MatrixFile;
using org.matheval;

namespace AvaloniaUi.Models;

class MatrixGenerator : IDisposable
{
    Expression itemExpression;
    Metadata matrixMetadata;
    public MatrixGenerator(string itemExpression, int sideSize)
    {
        this.itemExpression = new Expression(itemExpression);
        if (this.itemExpression.GetError().Count != 0)
        {
            throw new ArgumentException("expression have errors", nameof(itemExpression));
        }
        matrixMetadata = new Metadata()
        {
            Columns = sideSize,
            Rows = sideSize,
        };
    }

    public ProgressChangedEventHandler? ProgressEvent { get; set; }

    public async Task Generate(string filePath, CancellationToken cancellationToken)
    {
        var matrix = new Matrix(filePath, matrixMetadata, noFill: true);
        using(ItemsStream items = matrix.GetData())
        {
            var percent = int.Max(matrix.Rows / 100, 1);
            for (int i = 0; i < matrix.Rows; ++i)
            {
                if(i % percent == 0) {
                    ProgressEvent?.Invoke(this, new ProgressChangedEventArgs(i / percent, null));
                }
                for (int j = 0; j < matrix.Columns; ++j) {
                    itemExpression.Bind("i", i+1);
                    itemExpression.Bind("j", j+1);
                    itemExpression.Bind("kr", Convert.ToInt32(i == j));
                    var value = await Task.Run(itemExpression.Eval<int>);
                    await items.WriteItemAsync(value, cancellationToken);
                }
            }
        }
    }
    public void Dispose()
    {
        
    }
}