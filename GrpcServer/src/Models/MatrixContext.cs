using Microsoft.EntityFrameworkCore;

namespace GrpcServer.Models;

public class MatrixContext : DbContext
{
    public MatrixContext(DbContextOptions<MatrixContext> options)
        : base(options)
    {
    }

    public DbSet<Matrix> Matrix { get; set; } = null!;
}