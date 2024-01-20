using System.Diagnostics;

namespace MatrixFile.Bytes;
public class RowStream : Stream
{
    private readonly int row;
    private readonly long rowStart;
    private ItemsStream items;
    private Metadata Metadata;
    public override long Length
    {
        get
        {
            return Metadata.Columns * sizeof(float);
        }
    }

    private long _position;
    public override long Position 
    {
        get
        {
            return _position;
        }
        set
        {
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
        count = int.Min(count, (int)(Length - Position - offset));
        int read = items.Read(buffer, offset, count);
        Position += read;
        return read;
    }

    

    public RowStream(FileStream src, int row)
    {
        Metadata = Metadata.ReadFrom(src);
        items = new ItemsStream(src);
        this.row = row;
        rowStart = Length * this.row;
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
            _position = offset;
            return items.Seek(rowStart + offset, SeekOrigin.Begin);
        }
        if (origin == SeekOrigin.End) 
        {
            if (offset > 0 || offset < -Length)
            {
                throw new IndexOutOfRangeException("Out of row range");
            }
            _position = Length - offset;
            return items.Seek(rowStart + Length - offset, SeekOrigin.Begin);
        }
        if (origin == SeekOrigin.Current)
        {
            if (offset > (Length - Position) || offset < (rowStart - Position))
            {
                throw new IndexOutOfRangeException("Out of row range");
            }
            _position += offset;
            return items.Seek(offset, SeekOrigin.Current);
        }
        throw new UnreachableException();
    }

    public override void Flush() => items.Flush();
    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public new void Dispose() {
        base.Dispose();
        items.Close();
    }
}