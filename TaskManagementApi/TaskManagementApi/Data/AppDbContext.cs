using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Models;

namespace TaskManagementApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskItem>(e =>
            {
                e.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
                e.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);

                e.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(t => t.Assignee)
                  .WithMany(u => u.AssignedTasks)
                  .HasForeignKey(t => t.AssigneeId)
                  .OnDelete(DeleteBehavior.SetNull);
            });

        }
    
        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
    
    }
}
