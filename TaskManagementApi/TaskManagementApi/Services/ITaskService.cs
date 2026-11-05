using TaskManagementApi.DTOs;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services
{
    public interface ITaskService 
    {
        Task<PagedResult<TaskResponseDto>> GetTasks(
            TaskItemStatus? status = null,
            int? assigneeId = null,
            int page = 1,
            int pageSize = 10);
        Task<TaskResponseDto?> GetTaskById(int id);
        Task<TaskResponseDto?> UpdateTask(int id, UpdateTaskDto taskItem);
        Task<TaskResponseDto> CreateTask(CreateTaskDto createTask);
        Task<bool> DeleteTask(int id);
    }
}
