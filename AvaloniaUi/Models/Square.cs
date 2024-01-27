using System.Drawing;

namespace AvaloniaUi.Models;
public record Square
{
    public Point TopLeftPoint { get; set; }
    public int Side { get; set; }
    public Square(Point point, int sideLength)
    {
        TopLeftPoint = point;
        Side = sideLength;
    }

    public int GetArea()
    {
        return Side*Side;
    }

    public Rectangle ToRectangle()
    {
        return new Rectangle(TopLeftPoint, new Size(Side, Side));
    }

    public int Left => TopLeftPoint.X;
    public int Right => TopLeftPoint.X + Side;
    public int Up => TopLeftPoint.Y;
    public int Down => TopLeftPoint.Y + Side;
}