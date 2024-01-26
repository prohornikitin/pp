using System.Diagnostics;
using MatrixFile.Bytes;

namespace MatrixFile;

public class Matrix {
    public readonly string FilePath;
    private readonly Metadata metadata;
    public Metadata Metadata => metadata with {};
    public int Rows => metadata.Rows;
    public int Columns => metadata.Columns;
    private readonly FileAccess fileAccess;
    public Matrix(string filePath, FileAccess fileAccess = FileAccess.Read)
    {
        this.fileAccess = fileAccess;
        FilePath = filePath;
        using (var file = File.OpenRead(filePath))
        {
            metadata = Metadata.ReadFrom(file);
        }
    }

    public Matrix(string filePath, Metadata metadata, FileAccess fileAccess = FileAccess.Write, bool noMetadataWrite = false)
    {
        FilePath = filePath;
        this.metadata = metadata;
        this.fileAccess = fileAccess;
        if(noMetadataWrite) {
            return;
        }
        using (var file = File.OpenWrite(filePath))
        {
            metadata.WriteTo(file);
            for(long i = 0; i < metadata.Columns * metadata.Rows * sizeof(int); ++i)
            {
                file.WriteByte(0);
            }
        }
    }

    public FileStream OpenFile()
    {
        return fileAccess switch
        {
            FileAccess.Read => File.OpenRead(FilePath),
            FileAccess.Write => File.OpenWrite(FilePath),
            FileAccess.ReadWrite => new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None),
            _ => throw new UnreachableException(),
        };
    }

    public ItemsStream GetData() => new DataStream(OpenFile(), metadata);
    public ItemsStream GetRow(int row) => new RowStream(OpenFile(), row, metadata);
    public ItemsStream GetColumn(int column) => new ColumnStream(OpenFile(), column, metadata);

    public void Add(Matrix other)
    {
        //TODO: optimize with buffer;
        using(ItemsStream thisData = this.GetData(), otherData = other.GetData())
        {
            while (otherData.Position < otherData.Length)
            {
                var result = thisData.ReadItem() + otherData.ReadItem();
                thisData.SeekItem(-1, SeekOrigin.Current);
                thisData.WriteItem(result);
            }
        }
    }

    public static Matrix operator +(Matrix a, Matrix b)
    {
        if (a.Rows == b.Rows && a.Columns == b.Columns)
        {
            throw new ArgumentException();
        }
        //TODO: optimize with buffer;
        var result = new Matrix(Path.GetTempPath(), a.metadata);
        using(ItemsStream resultData = result.GetData(), aData = a.GetData(), bData = b.GetData())
        {
            while (aData.Position < aData.Length)
            {
                var resultItem = aData.ReadItem() + bData.ReadItem();
                resultData.WriteItem(resultItem);
            }
        }
        return result;
    }

    public void MultiplyBy(int k)
    {
        //TODO: optimize with buffer;
        using(var data = GetData()) {
            while (data.Position < data.Length)
            {
                var result = data.ReadItem() * k;
                data.SeekItem(-1, SeekOrigin.Current);
                data.WriteItem(result);
            }
        }
    }

    public static Matrix operator *(Matrix m, int k)
    {
        //TODO: optimize with buffer;
        var result = new Matrix(Path.GetTempFileName(), m.metadata, FileAccess.ReadWrite);
        using(ItemsStream resultData = result.GetData(), data = m.GetData())
        {
            while (data.Position < data.Length)
            {
                var resultItem = data.ReadItem() * k;
                resultData.WriteItem(resultItem);
            }
        }
        return result;
    }
}