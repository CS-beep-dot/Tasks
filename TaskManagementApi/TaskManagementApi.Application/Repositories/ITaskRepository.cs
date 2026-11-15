using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Application.Repositories
{
    public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(int id);
        Task<TaskItem?> GetByIdForUpdateAsync(int id);
        Task<(List<TaskItem> Items, int Total)> GetPageAsync(
            TaskItemStatus? status, int? assigneeId, int page, int pageSize);
        Task<bool> ProjectExistsAsync(int id);
        Task<bool> UserExistsAsync(int id);
        void Add(TaskItem task);
        void Remove(TaskItem task);
        Task SaveChangesAsync();
    }
}
