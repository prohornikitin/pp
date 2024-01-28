using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MatrixFile.Bytes;

namespace AvaloniaUi.Models;

public class MatrixMovingWindow : IMatrixSquareMovingWindow
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
                newWindow.Right > meta.Width || 
                newWindow.Down > meta.Height ||
                newWindow.Left < 0 ||
                newWindow.Up < 0
            )
            {
                return;
            }
            window = newWindow;
        }
    }

    public int SideLength {
        get => window.Side;
        set
        {
            window.Side = SideLength;
        }
    }

    public string FileName { get; }

    private Square window;
    private RectangleBlockStream src;
    private MatrixFile.Metadata meta;
    public MatrixMovingWindow(string filePath, int maxWindowSideLength)
    {
        FileName = Path.GetFileName(filePath);
        var matrix = new MatrixFile.Matrix(filePath);
        meta = matrix.Metadata;
        var minDimension = int.Min(meta.Columns, meta.Rows);
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