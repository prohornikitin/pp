using Metadata = MatrixFile.Metadata;

namespace Server.Models;
public class Matrix
{
    public static Matrix WithExistingFile(string filePath)
    {
        using var file = File.OpenRead(filePath);
        return new Matrix {
            FilePath = filePath,
            Metadata = Metadata.ReadFrom(file),
            Name = "",
        };
    }

    public static Matrix EmptyWithMetadata(string filePath, Metadata metadata)
    {
        var _ = new MatrixFile.Matrix(filePath, metadata);
        return new Matrix {
            FilePath = filePath,
            Metadata = metadata,
            Name = "",
        };
    }
    public long Id { get; set; }
    public required Metadata Metadata { get; set; }
    public required string FilePath { get; set; }
    public required string Name { get; set; }
}