using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Data;
using TaskManagementApi.DTOs;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services
{
    public class TaskService : ITaskService
    {

        //private readonly AppDbContext _db;

        //public TaskService(AppDbContext db)
        //{
        //    _db = db;
        //}

        private readonly List<TaskItem> _tasks = new();
        private int _nextId = 1;

        public IEnumerable<TaskResponseDto> GetTasks(TaskItemStatus? status = null)
        {
            return _tasks
                .Where(t => t.Status == status || status == null)
                .OrderBy(t => t.Id)
                .Select(ToResponse)
                .ToList();
        }

        public TaskResponseDto? GetTaskById(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return null;
            }

            return ToResponse(task);
        }

        public TaskResponseDto? UpdateTask(int id, UpdateTaskDto taskItem)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return null;
            }

            task.Description = taskItem.Description;
            task.Title = taskItem.Title;
            task.Priority = taskItem.Priority;
            task.DueDate = taskItem.DueDate;
            task.Status = taskItem.Status;

            return ToResponse(task);
        }

        public TaskResponseDto CreateTask(CreateTaskDto createTask)
        {
            var task = new TaskItem()
            {
                Id = _nextId++,
                Title = createTask.Title.Trim(),
                Description = createTask.Description,
                Priority = createTask.Priority,
                DueDate = createTask.DueDate,
                Status = createTask.Status
            };
            _tasks.Add(task);
            return ToResponse(task);
        }

        public bool DeleteTask(int id)
        {
            var task = _tasks.FirstOrDefault(_t => _t.Id == id);

            if ( task == null ) 
                return false;

            _tasks.Remove(task);
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
            Status = task.Status
        };
    }
}
