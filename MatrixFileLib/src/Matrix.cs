using MatrixFile.Bytes;

namespace MatrixFile;

public class MatrixFile : IDisposable {
    private FileStream file;
    private Metadata metadata;
    public int Rows => metadata.Rows;
    public int Column => metadata.Columns;
    private int ItemSize => sizeof(float);
    public MatrixFile(FileStream file) {
        this.file = file;
        metadata = Metadata.ReadFrom(file);
    }
    Stream Items => new ItemsStream(file);
    Stream GetRow(int row) => new RowStream(file, row);
    Stream GetColumn(int column) => new ColumnStream(file, column);
    public void Dispose()
    {
        file.Dispose();
    }
}