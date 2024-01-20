using System.Diagnostics;

namespace MatrixFile.Bytes;
public class ItemsStream : Stream
{
    private readonly long itemsStart;
    private Stream src;
    private Metadata Metadata;

    public int Rows => Metadata.Rows;
    public int Columns => Metadata.Columns;

    public override long Length
    {
        get
        {
            return Columns * Rows * sizeof(float);
        }
    }
    private long _position = 0;
    public override long Position
    {
        get => _position;
        set 
        {
            if(value != _position) {
                _position = value;
                Seek(itemsStart + value, SeekOrigin.Begin);
            }
        }
    }
    public override bool CanSeek => src.CanSeek;
    public override bool CanRead => src.CanRead;
    public override bool CanWrite => src.CanWrite;

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (count > (Length - Position - offset))
        {
            throw new IndexOutOfRangeException("count is too large");
        }
        src.Write(buffer, offset, count);
        _position += count;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        count = int.Min(count, (int)(Length - Position));
        var read = src.Read(buffer, offset, count);
        _position += read;
        return read;
    }

    public ItemsStream(FileStream src) : this(src, Metadata.ReadFrom(src))
    {
        
    }

    ItemsStream(FileStream src, Metadata metadata) {
        this.src = src;
        Metadata = metadata;
        itemsStart = Metadata.size;
        Position = 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin == SeekOrigin.Begin)
        {
            if (offset < 0 || offset > Length)
            {
                throw new IndexOutOfRangeException("Outside of the matrix data");
            }
            _position = offset;
            return src.Seek(itemsStart + offset, SeekOrigin.Begin);
        }
        if (origin == SeekOrigin.End) 
        {
            Seek(Length - offset, SeekOrigin.End);
        }
        if (origin == SeekOrigin.Current)
        {
            if (offset > (Length - Position) || offset < (Position - Length))
            {
                throw new IndexOutOfRangeException("Outside of the matrix data");
            }
            _position += offset;
            return src.Seek(offset, SeekOrigin.Current);
        }
        throw new UnreachableException();
    }

    public override void Flush() => src.Flush();
    public override void SetLength(long value) => throw new NotImplementedException();

    public new void Dispose() {
        base.Dispose();
        src.Close();
    }
}
