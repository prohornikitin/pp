using System;
using MatrixFile;
using MatrixFile.Bytes;
using org.matheval;

namespace AvaloniaUi.Models;

class MatrixGenerator
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
    public async void Generate(string filePath)
    {
        var matrix = new Matrix(filePath, matrixMetadata, noFill: true);
        using(ItemsStream items = matrix.GetData())
        {
            for (int i = 0; i < matrix.Rows; ++i)
            {
                for (int j = 0; j < matrix.Columns; ++j) {
                    itemExpression.Bind("i", i+1);
                    itemExpression.Bind("j", j+1);
                    itemExpression.Bind("kr", Convert.ToInt32(i == j));
                    await items.WriteItemAsync(itemExpression.Eval<int>());
                }
            }
        }
    }
}