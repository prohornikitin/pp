using System.Diagnostics;

namespace MatrixFile.Bytes;
public class DataStream : ItemsStream
{
    private readonly long itemsStart;
    private Stream src;
    private Metadata metadata;
    public Metadata Metadata => metadata with { };

    public int Rows => metadata.Rows;
    public int Columns => metadata.Columns;

    public override long Length
    {
        get
        {
            return Columns * Rows * itemSize;
        }
    }
    private long _position = 0;
    public override long Position
    {
        get => _position;
        set
        {
            _position = value;
            Seek(value, SeekOrigin.Begin);
        }
    }
    public override bool CanSeek => src.CanSeek;
    public override bool CanRead => src.CanRead;
    public override bool CanWrite => src.CanWrite;

    public override void Write(byte[] buffer, int offset, int count)
    {
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

    public DataStream(FileStream src) : this(src, Metadata.ReadFrom(src))
    {

    }

    public DataStream(FileStream src, Metadata metadata)
    {
        this.src = src;
        // this.src = new BufferedStream(src, 4096 * 32);
        this.metadata = metadata;
        itemsStart = Metadata.Size;
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
            if (offset > 0 || offset < -Length)
            {
                throw new IndexOutOfRangeException("Outside of the matrix data");
            }
            _position = Length - offset;
            Seek(Length - offset, SeekOrigin.End);
        }
        if (origin == SeekOrigin.Current)
        {
            if (offset > (Length - Position) || offset < -Position)
            {
                throw new IndexOutOfRangeException("Outside of the matrix data");
            }
            _position += offset;
            return src.Seek(offset, SeekOrigin.Current);
        }
        throw new UnreachableException();
    }

    public override void Flush() => src.Flush();
    public override void SetLength(long value) => SetLength(itemsStart + value);

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            src.Dispose();
        }
    }
}
