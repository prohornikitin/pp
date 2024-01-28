using System.Collections.Generic;
using System.Drawing;

namespace AvaloniaUi.Models;

public class NullMatrixMovingWindow : IMatrixSquareMovingWindow
{
    public string FileName => "No File";
    private Point location;
    public Point Location { 
        get => location; 
        set {
            if(value.X < 0 || value.Y < 0) {
                return;
            }
            location = value;
        }
    }

    public int SideLength { get; set;}
    private int[] zeros;
    public NullMatrixMovingWindow(int windowSideLength)
    {
        SideLength = windowSideLength;
        zeros = new int[SideLength * SideLength];
    }

    public IList<int> GetWindowContent()
    {
        return zeros;
    }

    public void Dispose()
    {
    }
}