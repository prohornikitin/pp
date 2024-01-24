using Microsoft.EntityFrameworkCore;

namespace Server.Models;

public class TheOnlyDbContext : DbContext
{
    public TheOnlyDbContext(DbContextOptions<TheOnlyDbContext> options)
        : base(options)
    {
    }

    public DbSet<NodeTask> NodeTasks { get; set; } = null!;
    public DbSet<UserTask> UserTasks { get; set; } = null!;
    public DbSet<Matrix> Matrices { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserTask>()
            .HasOne(t => t.InitialMatrix)
            .WithMany()
            .HasForeignKey(e => e.InitialMatrixId)
            .IsRequired();
        modelBuilder.Entity<UserTask>()
            .HasOne(t => t.Result)
            .WithMany()
            .HasForeignKey(t => t.ResultMatrixId);
        modelBuilder.Entity<NodeTask>()
            .HasOne(t => t.UserTask)
            .WithMany()
            .HasForeignKey(t => t.UserTaskId);
    }
}