namespace MatrixFile;

public readonly record struct Metadata(int Rows, int Columns)
{
    private static readonly byte[] signature = {0x24, 0x27, 0x10, 0x15};
    public int Width => Columns;
    public int Height => Rows;
    
    public static long Size
    {
        get
        {
            return 2*sizeof(int) + signature.Length;
        }
    }

    internal static Metadata ReadFrom(Stream source)
    {
        source.Seek(0, SeekOrigin.Begin);
        int rows, columns;
        ReadAndCheckSignature(source);
        var reader = new BinaryReader(source);
        rows = reader.ReadInt32();
        columns = reader.ReadInt32();
        return new(rows, columns);
    }

    private static void ReadAndCheckSignature(Stream reader)
    {
        byte[] buffer = new byte[signature.Length];
        reader.Read(buffer);
        if(!buffer.SequenceEqual(signature)) {
            throw new ArgumentException("Not a matrix file. Signature not found");
        }
    }

    internal void WriteTo(Stream destination) {
        destination.Seek(0, SeekOrigin.Begin);
        var writer = new BinaryWriter(destination);
        writer.Write(signature);
        writer.Write(Rows);
        writer.Write(Columns);
    }
}
