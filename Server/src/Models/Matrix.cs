namespace Server.Models;
public class Matrix
{
    public static Matrix WithExistingFile(string FilePath)
    {
        using var file = File.OpenRead(FilePath);
        return new Matrix {
            FilePath = FilePath,
            Columns = MatrixFile.Metadata.ReadFrom(file).Columns,
        };
    }
    public long Id { get; set; }
    public required int Columns { get; set; }
    public required string FilePath { get; set; }
}