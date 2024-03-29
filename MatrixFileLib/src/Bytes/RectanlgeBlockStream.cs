using System.Diagnostics;
using System.Drawing;

namespace MatrixFile.Bytes;
public class RectangleBlockStream : ItemsStream
{
    private Stream data;
    private Rectangle _rectangle;
    public Rectangle Rectangle {
        get => _rectangle;
        set
        {
            _rectangle = value;
            Seek(0, SeekOrigin.Begin);
        }
    }
    private readonly Metadata metadata;

    public Size Size => Rectangle.Size;

    public override long Length
    {
        get
        {
            return Rectangle.Height * Rectangle.Width * itemSize;
        }
    }
    private long _position = 0;

    public override long Position
    {
        get => _position;
        set
        {
            Seek(value, SeekOrigin.Begin);
            _position = value;
        }
    }
    public override bool CanSeek => data.CanSeek;
    public override bool CanRead => data.CanRead;
    public override bool CanWrite => data.CanWrite;

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (metadata.Columns == 1 || Rectangle.Width == metadata.Columns)
        {
            data.Write(buffer, offset, count);
            _position += count;
            return;
        }
        count = (int)long.Min(count, Length - Position);
        int bytesInRow = Rectangle.Width * itemSize;

        int bytesBeforeLineBreak = (int)(bytesInRow - Position % bytesInRow) % bytesInRow;
        data.Write(buffer, offset, bytesBeforeLineBreak);
        count -= bytesBeforeLineBreak;
        Position += bytesBeforeLineBreak;

        int rowsInIntegralPart = count / bytesInRow;
        int bytesInIntegralPart = rowsInIntegralPart * bytesInRow;
        for (int i = 0; i < rowsInIntegralPart; ++i)
        {
            data.Write(buffer, offset + bytesBeforeLineBreak + i * bytesInRow, bytesInRow);
            Position += bytesInRow;
        }
        count -= bytesInIntegralPart;

        data.Write(buffer, offset + bytesBeforeLineBreak + bytesInIntegralPart, count);
        Position += count;
    }



    public override int Read(byte[] buffer, int offset, int count)
    {
        if (metadata.Columns == 1 || Rectangle.Width == metadata.Columns)
        {
            int read = data.Read(buffer, offset, count);
            _position += read;
            return read;
        }
        count = (int)long.Min(count, Length - Position);
        int bytesInRow = Rectangle.Width * itemSize;

        int bytesBeforeLineBreak = int.Min((int)((bytesInRow - Position % bytesInRow) % bytesInRow), count);
        int readBeforeLineBreak = data.Read(buffer, offset, bytesBeforeLineBreak);
        count -= bytesBeforeLineBreak;
        Position += readBeforeLineBreak;

        int rowsInIntegralPart = count / bytesInRow;
        int bytesInIntegralPart = rowsInIntegralPart * bytesInRow;
        int readInIntegralPart = 0;
        for (int i = 0; i < rowsInIntegralPart; ++i)
        {
            int read = data.Read(buffer, offset + bytesBeforeLineBreak + i * bytesInRow, bytesInRow);
            Position += read;
            readInIntegralPart += read;
        }
        count -= bytesInIntegralPart;

        int readInLastLine = data.Read(buffer, offset + bytesBeforeLineBreak + bytesInIntegralPart, count);
        Position += readInLastLine;
        return readBeforeLineBreak + readInIntegralPart + readInLastLine;
    }

    public RectangleBlockStream(FileStream src, Rectangle rectangle)
        : this(src, rectangle, Metadata.ReadFrom(src))
    {

    }
    public RectangleBlockStream(FileStream src, Rectangle rectangle, Metadata metadata)
    {
        this._rectangle = rectangle;
        this.metadata = metadata;
        var fullMatrix = new Rectangle(new Point(0, 0), new Size(metadata.Columns, metadata.Rows));
        if (rectangle.IsEmpty)
        {
            throw new ArgumentOutOfRangeException($"rectangle must not be empty");
        }
        if (!fullMatrix.Contains(rectangle))
        {
            throw new ArgumentOutOfRangeException($"rectangle {rectangle} goes beyong the matrix");
        }
        data = new DataStream(src, metadata);
        Position = 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin == SeekOrigin.Begin)
        {
            if (offset < 0 || offset > Length)
            {
                throw new IndexOutOfRangeException($"{offset} is out of rectangle");
            }
            long row = offset / (Rectangle.Width * itemSize) + Rectangle.Top;
            long column = offset % (Rectangle.Width * itemSize) / itemSize + Rectangle.Left;
            long remains = offset % itemSize;
            long positionInItems = row * metadata.Width + column;
            long newPosition = positionInItems * itemSize + remains;
            _position = offset;
            if (newPosition > data.Length)
            {
                return data.Seek(Length, SeekOrigin.Begin);
            }
            return data.Seek(newPosition, SeekOrigin.Begin);
        }
        if (origin == SeekOrigin.End)
        {
            Seek(Length - offset, SeekOrigin.Begin);
        }
        if (origin == SeekOrigin.Current)
        {
            Seek(Position + offset, SeekOrigin.Begin);
        }
        throw new UnreachableException();
    }

    public override void Flush() => data.Flush();
    public override void SetLength(long value) => throw new NotImplementedException();

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            data.Dispose();
        }
    }
}
