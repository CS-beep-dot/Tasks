using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Infrastructure.Persistence
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

            // Test Data
            // Values must be constant (no DateTime.Now) or EF creates a new migration every time.
            modelBuilder.Entity<Project>().HasData(
                new Project { Id = 1, Name = "Website", Description = "Company website redesign", CreatedAt = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Project { Id = 2, Name = "Mobile App", Description = "iOS and Android app", CreatedAt = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Nino", Email = "nino@gmail.com" },
                new User { Id = 2, Name = "Giorgi", Email = "giorgi@gmail.com" }
            );

        }
    
        public DbSet<User> Users => Set<User>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
    
    }
}
