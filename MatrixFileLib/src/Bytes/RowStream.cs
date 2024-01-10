using System.Diagnostics;
using MatrixFile;

namespace MatrixFile.Bytes;
public class RowStream : Stream
{
    private readonly int rowIndex;
    private readonly long rowStartInFile;
    private FileStream file;
    private Metadata metadata;
    public override long Length
    {
        get
        {
            return metadata.columns;
        }
    }
    private long Size
    {
        get
        {
            return Length * sizeof(int);
        }
    }
    public override long Position {get; set;}
    public override bool CanSeek => file.CanSeek;
    public override bool CanRead => file.CanRead;
    public override bool CanWrite => file.CanWrite;

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (count > (Size - Position - offset))
        {
            throw new IndexOutOfRangeException("count is too large");
        }
        file.Write(buffer, offset, count);
        Position += count;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (count > (Size - Position - offset))
        {
            throw new IndexOutOfRangeException("count is too large");
        }
        file.ReadExactly(buffer, offset, count);
        Position += count;
        return count;
    }

    public RowStream(FileStream file, int row)
    {
        this.file = file;
        rowIndex = row;
        Position = 0;
        metadata = Metadata.ReadFrom(file);
        rowStartInFile = Length * rowIndex + Metadata.size;
        file.Seek(rowStartInFile, SeekOrigin.Begin);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (origin == SeekOrigin.Begin)
        {
            if (offset < 0 || offset > Size)
            {
                throw new IndexOutOfRangeException("Out of row range");
            }
            return file.Seek(rowStartInFile + offset, SeekOrigin.Begin);
        }
        if (origin == SeekOrigin.End) 
        {
            if (offset > 0 || offset < -Size)
            {
                throw new IndexOutOfRangeException("Out of row range");
            }
            return file.Seek(rowStartInFile + Size - offset, SeekOrigin.Begin);
        }
        if (origin == SeekOrigin.Current)
        {
            if (offset > (Length - Position) * sizeof(int) || offset < (Position - Length) * sizeof(int))
            {
                throw new IndexOutOfRangeException("Out of row range");
            }
            return file.Seek(offset, SeekOrigin.Current);
        }
        throw new UnreachableException("new SeekOrigin???");
    }

    public override void Flush() => file.Flush();
    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }
}
