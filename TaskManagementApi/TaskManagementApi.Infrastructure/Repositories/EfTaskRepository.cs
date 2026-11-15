using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Domain.Entities;
using TaskManagementApi.Application.Repositories;
using TaskManagementApi.Infrastructure.Persistence;

namespace TaskManagementApi.Infrastructure.Repositories
{
    public class EfTaskRepository : ITaskRepository
    {
        private readonly AppDbContext _db;

        public EfTaskRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem?> GetByIdForUpdateAsync(int id)
        {
            return await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<(List<TaskItem> Items, int Total)> GetPageAsync(
            TaskItemStatus? status, int? assigneeId, int page, int pageSize)
        {
            IQueryable<TaskItem> query = _db.Tasks.AsNoTracking();

            if (status != null)
                query = query.Where(t => t.Status == status);

            if (assigneeId != null)
                query = query.Where(t => t.AssigneeId == assigneeId);

            int total = await query.CountAsync();

            var items = await query
                .OrderBy(t => t.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }


        public Task<bool> ProjectExistsAsync(int id)
        {
            return _db.Projects.AnyAsync(p => p.Id == id);
        }

        public Task<bool> UserExistsAsync(int id)
        {
            return  _db.Users.AnyAsync(u => u.Id == id);
        }

        public void Add(TaskItem task) => _db.Tasks.Add(task);

        public void Remove(TaskItem task) => _db.Tasks.Remove(task);

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
