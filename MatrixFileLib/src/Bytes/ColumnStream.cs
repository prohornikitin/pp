using System.Drawing;

namespace MatrixFile.Bytes;
public class ColumnStream : RectangleBlockStream
{
    public ColumnStream(FileStream src, int column, Metadata metadata) : base(
        src, 
        new Rectangle(
            new Point(column, 0),
            new Size(1, metadata.Rows)
        ),
        metadata
    )
    {
    }

    public ColumnStream(FileStream src, int column) : this(
        src, 
        column,
        Metadata.ReadFrom(src)
    )
    {
    }
}
