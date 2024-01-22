using Microsoft.EntityFrameworkCore;

namespace GrpcServer.Models;

public class UserTaskContext : DbContext
{
    public UserTaskContext(DbContextOptions<UserTaskContext> options)
        : base(options)
    {
    }

    public DbSet<UserTask> Tasks { get; set; } = null!;
}