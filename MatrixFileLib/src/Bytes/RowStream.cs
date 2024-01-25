using System.Drawing;

namespace MatrixFile.Bytes;
public class RowStream : RectangleBlockStream
{
    public RowStream(FileStream src, int row, Metadata metadata) : base(
        src, 
        new Rectangle(
            new Point(0, row),
            new Size(metadata.Columns, 1)
        )
    )
    {
    }

    public RowStream(FileStream src, int row) : this(
        src, 
        row,
        Metadata.ReadFrom(src)
    )
    {
    }
}
