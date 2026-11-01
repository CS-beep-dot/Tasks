using TaskManagementApi.DTOs;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services
{
    public interface ITaskService
    {
        IEnumerable<TaskResponseDto> GetTasks();
        TaskResponseDto? GetTaskById(int id);
        TaskResponseDto? UpdateTask(int id, UpdateTaskDto taskItem);
        TaskResponseDto CreateTask(CreateTaskDto createTask);
    }
}
