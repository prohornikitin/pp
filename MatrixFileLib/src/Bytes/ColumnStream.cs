using System.Diagnostics;

namespace MatrixFile.Bytes;
public class ColumnStream : ItemsStream
{
    private readonly int column;
    private Stream items;
    private Metadata metadata;
    public Metadata Metadata => metadata with {};

    public override long Length
    {
        get
        {
            return metadata.Rows * itemSize;
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
        if (metadata.Columns == 1) {
            items.Write(buffer, offset, count);
        }
        count = int.Min(count, (int)(Length - Position));
        int before = (int)(itemSize - Position % itemSize) % itemSize;
        items.Write(buffer, offset, before);
        count -= before;
        Position += before;
        
        var integralPart = count / itemSize * itemSize;
        for(int i=0; i < integralPart / itemSize; ++i)
        {
            items.Write(buffer, offset + before + i * itemSize, itemSize);
            Position += itemSize;
        }
        count -= integralPart;
        
        var after = int.Min(count, (int)(Length - Position));
        items.Write(buffer, offset + before + integralPart, after);
        Position += after;
    }

   

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (metadata.Columns == 1) {
            return items.Read(buffer, offset, count);
        }
        count = int.Min(count, (int)(Length - Position));
        int before = (int)(itemSize - Position % itemSize) % itemSize;
        var readBefore = items.Read(buffer, offset, before);
        count -= before;
        Position += readBefore;
    
        var integralPart = count / itemSize * itemSize;
        var readIntegralPart = 0;
        for(int i=0; i < integralPart / itemSize; ++i)
        {
            readIntegralPart += items.Read(buffer, offset + readBefore + i * itemSize, itemSize);
            Position += itemSize;
        }
        count -= integralPart;
        
        var after = int.Min(count, (int)(Length - Position));
        var readAfter = items.Read(buffer, offset + readBefore + readIntegralPart, after);
        Position += readAfter;        
        return readBefore + readIntegralPart + readAfter;
    }

    public ColumnStream(FileStream src, int column)
    {
        metadata = Metadata.ReadFrom(src);
        items = new DataStream(src);
        this.column = column;
        Position = 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin == SeekOrigin.Begin)
        {
            if (offset < 0 || offset > Length)
            {
                throw new IndexOutOfRangeException($"{offset} is out of column range");
            }
            var row = offset / itemSize;
            var remains = offset % itemSize;
            var positionInItems = row * metadata.Columns + column;
            _position = offset;
            var newPosition = positionInItems * itemSize + remains;
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

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        if(disposing)
        {
            items.Dispose();
        }
    }
}
