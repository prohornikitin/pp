using System.Diagnostics;

class MatrixFileColumnStream : Stream
{
    private readonly int columnIndex;
    private FileStream file;
    private Metadata metadata;
    private readonly long columnStartInFile;

    private long RowLength
    {
        get
        {
            return metadata.columns;
        }
    }
    
    public override long Length
    {
        get
        {
            return metadata.rows;
        }
    }
    public override long Position {get; set;}
    public override bool CanSeek => file.CanSeek;
    public override bool CanRead => file.CanRead;
    public override bool CanWrite => file.CanWrite;

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (count > (Length - Position) * sizeof(int))
        {
            throw new IndexOutOfRangeException("count is too large");
        }
        //TODO:
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (count > (Length - Position) * sizeof(int))
        {
            throw new IndexOutOfRangeException("count is too large");
        }
        var rowSize = Length * sizeof(int);
        for(int i = 0; i < count / sizeof(int); ++i)
        {
            file.Seek(Metadata.size + columnIndex + i*rowSize, SeekOrigin.Begin);
            file.ReadExactly(buffer, i*sizeof(int), sizeof(int));
            Position += sizeof(int);
        }
        int remains = count % sizeof(int);
        file.Seek(Metadata.size + columnIndex + Position / sizeof(int) * sizeof(int) * rowSize, SeekOrigin.Begin);
        file.ReadExactly(buffer, count / sizeof(int) * sizeof(int), remains);
        Position += remains;
        return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        long columnSize = Length * sizeof(int);
        long rowSize = RowLength * sizeof(int);
        if (origin == SeekOrigin.Begin)
        {
            if (offset < 0 || offset > columnSize)
            {
                throw new IndexOutOfRangeException("Out of row range");
            }
            Position = offset;
            return file.Seek(columnStartInFile + offset/sizeof(int) * rowSize + offset%sizeof(int), SeekOrigin.Begin);
        }
        if (origin == SeekOrigin.End) 
        {
            if (offset > 0 || offset < columnSize)
            {
                throw new IndexOutOfRangeException("Out of row range");
            }
            Position = rowSize - offset;
            return file.Seek(offset/sizeof(int) * rowSize + offset%sizeof(int), SeekOrigin.End);
        }
        if (origin == SeekOrigin.Current)
        {
            if (offset > (Length - Position) * sizeof(int) || offset < (Position - Length) * sizeof(int))
            {
                throw new IndexOutOfRangeException("Out of row range");
            }
            Position += offset;
            return file.Seek(columnStartInFile + Position/sizeof(int) * rowSize + Position%sizeof(int), SeekOrigin.Current);
        }
        throw new UnreachableException("new SeekOrigin???");
    }

    public MatrixFileColumnStream(FileStream file, int column)
    {
        this.file = file;
        columnIndex = column;
        Position = 0;
        metadata = Metadata.ReadFrom(file);
        columnStartInFile = RowLength * sizeof(int) * columnIndex + Metadata.size;
        file.Seek(columnStartInFile, SeekOrigin.Begin);
    }

    public override void Flush() => file.Flush();
    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }
}

