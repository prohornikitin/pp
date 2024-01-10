
namespace MatrixFile;
public readonly record struct Metadata(int rows, int columns) {
    public static long size {
        get {
            return 2*sizeof(int);
        }
    }

    public static Metadata ReadFrom(FileStream source)
    {
        source.Seek(0, SeekOrigin.Begin);
        var reader = new BinaryReader(source);
        int rows, columns;
        rows = reader.ReadInt32();
        columns = reader.ReadInt32();
        return new(rows, columns);
    }
}
