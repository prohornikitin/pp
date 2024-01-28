using System;
using System.Collections.Generic;
using System.Drawing;

namespace AvaloniaUi.Models;

public interface IMatrixSquareMovingWindow : IDisposable
{
    public abstract Point Location { get; set; }

    public int SideLength { get; set; }
    public string FileName { get; }
    public abstract IList<int> GetWindowContent();

    public void TryMoveWindow(int dx = 0, int dy = 0)
    {
        Location = new Point() {
            X = Location.X + dx,
            Y = Location.Y + dy,
        };
    }
}