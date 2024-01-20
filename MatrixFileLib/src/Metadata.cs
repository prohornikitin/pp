
using System.Diagnostics;
using static MatrixFile.Metadata;

namespace MatrixFile;

public readonly record struct Metadata(int Rows, int Columns)
{
    
    public static long size
    {
        get
        {
            return 2*sizeof(int);
        }
    }

    public static Metadata ReadFrom(Stream source)
    {
        source.Seek(0, SeekOrigin.Begin);
        int rows, columns;
        var reader = new BinaryReader(source);
        rows = reader.ReadInt32();
        columns = reader.ReadInt32();
        return new(rows, columns);
    }

    public void WriteTo(Stream destination) {
        destination.Seek(0, SeekOrigin.Begin);
        var writer = new BinaryWriter(destination);
        writer.Write(Rows);
        writer.Write(Columns);
    }
}
