using TaskManagementApi.Application.DTOs;
using TaskManagementApi.Application.Exceptions;
using TaskManagementApi.Domain.Entities;
using TaskManagementApi.Application.Repositories;

namespace TaskManagementApi.Application.Services
{
    public class TaskService : ITaskService
    {

        private readonly ITaskRepository _repo;

        public TaskService(ITaskRepository repo)
        {
            _repo = repo;
        }

        //Helper for 
        private async Task EnsureReferencesExistAsync(int projectId, int? assigneeId)
        {
            if (!await _repo.ProjectExistsAsync(projectId))
                throw new InvalidReferenceException($"Project {projectId} does not exist.");

            if (assigneeId != null && !await _repo.UserExistsAsync(assigneeId.Value))
                throw new InvalidReferenceException($"User {assigneeId} does not exist.");
        }


        public async Task<PagedResult<TaskResponseDto>> GetTasks(
            TaskItemStatus? status = null,
            int? assigneeId = null,
            int page = 1,
            int pageSize = 10)
        {

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var (items, total) = await _repo.GetPageAsync(status, assigneeId, page, pageSize);

            return new PagedResult<TaskResponseDto>
            {
                Items = items.Select(ToResponse).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<TaskResponseDto?> GetTaskById(int id)
        {
            var task = await _repo.GetByIdAsync(id);

            if (task == null)
                return null;

            return ToResponse(task);
        }
        public async Task<TaskResponseDto?> UpdateTask(int id, UpdateTaskDto taskItem)
        {
            await EnsureReferencesExistAsync(taskItem.ProjectId, taskItem.AssigneeId);

            var task = await _repo.GetByIdForUpdateAsync(id);

            if (task == null)
                return null;

            task.Description = taskItem.Description;
            task.Title = taskItem.Title;
            task.Priority = taskItem.Priority;
            task.DueDate = taskItem.DueDate!.Value;
            task.Status = taskItem.Status;
            task.AssigneeId = taskItem.AssigneeId;
            task.ProjectId = taskItem.ProjectId;

            await _repo.SaveChangesAsync();
            return ToResponse(task);
        }

        public async Task<TaskResponseDto> CreateTask(CreateTaskDto createTask)
        {
            await EnsureReferencesExistAsync(createTask.ProjectId, createTask.AssigneeId);
            var task = new TaskItem()
            {
                Title = createTask.Title.Trim(),
                Description = createTask.Description,
                Priority = createTask.Priority,
                DueDate = createTask.DueDate!.Value,
                Status = createTask.Status,
                ProjectId = createTask.ProjectId,
                AssigneeId = createTask.AssigneeId
            };
            
            _repo.Add(task);
            await _repo.SaveChangesAsync();

            return ToResponse(task);
        }

        public async Task<bool> DeleteTask(int id)
        {
            var task = await _repo.GetByIdForUpdateAsync(id);
            
            if ( task == null ) 
                return false;

            _repo.Remove(task);
            await _repo.SaveChangesAsync();

            return true;
        }

        //this returns TaskResponseDto for our responses on client's side
        private static TaskResponseDto ToResponse(TaskItem task) => new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            DueDate = task.DueDate,
            Status = task.Status,
            ProjectId = task.ProjectId,
            AssigneeId = task.AssigneeId
        };
    }
}
