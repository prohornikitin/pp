using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MatrixFile.Bytes;

namespace AvaloniaUi.Models;

public class FileMatrixMovingWindow : IMatrixSquareMovingWindow
{
    public Point Location
    {
        get => window.TopLeftPoint;
        set
        {
            var newWindow = window with {
                TopLeftPoint = value
            };
            if (
                newWindow.Right > matrixMetadata.Width || 
                newWindow.Down > matrixMetadata.Height ||
                newWindow.Left < 0 ||
                newWindow.Up < 0
            )
            {
                return;
            }
            window = newWindow;
        }
    }

    public int SideLength => window.Side;

    private Square window;
    private RectangleBlockStream src;
    private MatrixFile.Metadata matrixMetadata;
    public FileMatrixMovingWindow(MatrixFile.Matrix matrix, int maxWindowSideLength)
    {
        matrixMetadata = matrix.Metadata;
        var minDimension = int.Min(matrix.Metadata.Columns, matrix.Metadata.Rows);
        window = new Square(new Point(0,0), int.Min(maxWindowSideLength, minDimension));
        src = matrix.GetRectangleBlock(window.ToRectangle());
    }

    public IList<int> GetWindowContent()
    {
        src.Rectangle = window.ToRectangle();
        int[] content = new int[window.GetArea()];
        var binaryReader = new BinaryReader(src);
        for(int i = 0; i < content.Length; ++i)
        {
            content[i] = binaryReader.ReadInt32();
        }
        return content;
    }

    public void TryMoveWindow(int dx = 0, int dy = 0)
    {
        Location = new Point() {
            X = Location.X + dx,
            Y = Location.Y + dy,
        };
    }

    public void Dispose()
    {
        src.Dispose();
    }
}