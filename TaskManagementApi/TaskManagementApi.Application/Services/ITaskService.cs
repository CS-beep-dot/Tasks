using TaskManagementApi.Application.DTOs;
using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Application.Services
{
    public interface ITaskService 
    {
        Task<PagedResult<TaskResponseDto>> GetTasks(
            TaskItemStatus? status = null,
            int? assigneeId = null,
            TaskPriority? priority = null,
            int? projectId = null,
            string? search = null,
            string? sort = null,
            int page = 1,
            int pageSize = 20);
        Task<TaskResponseDto?> GetTaskById(int id);
        Task<TaskResponseDto?> UpdateTask(int id, UpdateTaskDto taskItem);
        Task<TaskResponseDto> CreateTask(CreateTaskDto createTask);

        Task<TaskResponseDto?> AssignTask(int id, int? assigneeId);
        Task<bool> DeleteTask(int id);
    }
}
