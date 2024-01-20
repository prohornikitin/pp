using System.Diagnostics;

namespace MatrixFile.Bytes;
public class ColumnStream : Stream
{
    private readonly int column;
    private Stream items;
    public Metadata Metadata;
    private int ItemSize => sizeof(float);

    public override long Length
    {
        get
        {
            return Metadata.Rows * ItemSize;
        }
    }
    private long _position = 0;

    public override long Position {
        get => _position;
        set {
            Seek(value, SeekOrigin.Begin);
            _position = value;
        }
    }
    public override bool CanSeek => items.CanSeek;
    public override bool CanRead => items.CanRead;
    public override bool CanWrite => items.CanWrite;

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (count > (Length - Position - offset))
        {
            throw new IndexOutOfRangeException("count is too large");
        }
        items.Write(buffer, offset, count);
        Position += count;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if(Metadata.Columns == 1) {
            return items.Read(buffer, offset, count);
        }
        count = int.Min(count, (int)(Length - Position));
        int before = (int)(ItemSize - Position % ItemSize) % ItemSize;
        var readBefore = items.Read(buffer, offset, before);
        count -= before;
        Position += readBefore;
        // Console.WriteLine($"attempt={before}, read={readBefore}");
    
        var integralPart = count / ItemSize * ItemSize;
        Console.WriteLine($"attempt={integralPart}");
        var readIntegralPart = 0;
        for(int i=0; i < integralPart / ItemSize; ++i) {
            readIntegralPart += items.Read(buffer, offset + readBefore + i * ItemSize, ItemSize);
            Position += ItemSize;
        }
        count -= integralPart;
        Console.WriteLine($"attempt={integralPart}, read={readIntegralPart}");
        
        var after = int.Min(count, (int)(Length - Position));
        var readAfter = items.Read(buffer, offset + readBefore + readIntegralPart, after);
        Position += readAfter;
        // Console.WriteLine($"attempt={after}, read={readAfter}");
        
        return readBefore + readIntegralPart + readAfter;
    }

    public ColumnStream(FileStream src, int column)
    {
        Metadata = Metadata.ReadFrom(src);
        items = new ItemsStream(src);
        this.column = column;
        Position = 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin == SeekOrigin.Begin)
        {
            if (offset < 0 || offset > Length)
            {
                throw new IndexOutOfRangeException("Out of row range");
            }
            var row = offset / ItemSize;
            var remains = offset % ItemSize;
            var positionInItems = row * Metadata.Columns + column;
            _position = offset;
            var newPosition = positionInItems * ItemSize + remains;
            if (newPosition > items.Length) {
                return items.Seek(Length, SeekOrigin.Begin);
            }
            return items.Seek(newPosition, SeekOrigin.Begin);
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

    public override void Flush() => items.Flush();
    public override void SetLength(long value) => throw new NotImplementedException();

    public new void Dispose() {
        base.Dispose();
        items.Close();
    }
}
