using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.DTOs;
using TaskManagementApi.Exceptions;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services
{
    public class TaskService : ITaskService
    {

        private readonly AppDbContext _db;

        public TaskService(AppDbContext db)
        {
            _db = db;
        }

        //Helper for 
        private async Task EnsureReferencesExistAsync(int projectId, int? assigneeId)
        {
            if (!await _db.Projects.AnyAsync(p => p.Id == projectId))
                throw new InvalidReferenceException($"Project {projectId} does not exist.");

            if (assigneeId != null && !await _db.Users.AnyAsync(u => u.Id == assigneeId))
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

            IQueryable<TaskItem> query = _db.Tasks.AsNoTracking();

            if (status != null)
            {
                query = query.Where(t => t.Status == status);
            }

            if (assigneeId != null)
            {
                query = query.Where(t => t.AssigneeId == assigneeId);
            }

            int total = await query.CountAsync();

            var tasks = await query
                .OrderBy(t => t.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Priority = t.Priority,
                    ProjectId = t.ProjectId,
                    Status = t.Status,
                    AssigneeId = t.AssigneeId,
                })
                .ToListAsync();

            return new PagedResult<TaskResponseDto>
            {
                Items = tasks,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<TaskResponseDto?> GetTaskById(int id)
        {
            var task = await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return null;

            return ToResponse(task);
        }
        public async Task<TaskResponseDto?> UpdateTask(int id, UpdateTaskDto taskItem)
        {
            await EnsureReferencesExistAsync(taskItem.ProjectId, taskItem.AssigneeId);

            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return null;

            task.Description = taskItem.Description;
            task.Title = taskItem.Title;
            task.Priority = taskItem.Priority;
            task.DueDate = taskItem.DueDate!.Value;
            task.Status = taskItem.Status;
            task.AssigneeId = taskItem.AssigneeId;
            task.ProjectId = taskItem.ProjectId;

            await _db.SaveChangesAsync();
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
            
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return ToResponse(task);
        }

        public async Task<bool> DeleteTask(int id)
        {
            var task = await _db.Tasks.FirstOrDefaultAsync(_t => _t.Id == id);
            
            if ( task == null ) 
                return false;

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();

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
