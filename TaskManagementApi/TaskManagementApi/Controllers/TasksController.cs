using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.DTOs;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace TaskManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }


        [HttpGet]
        public ActionResult<IEnumerable<TaskResponseDto>> GetAllTasks([FromQuery] TaskItemStatus? status)
        {
            return Ok(_taskService.GetTasks(status));
        }

        [HttpGet("{id}")]
        public ActionResult<TaskResponseDto> GetTaskById(int id)
        {
            var task = _taskService.GetTaskById(id);

            if (task is null)
                return NotFound();

            return Ok(task);
        }

        [HttpPut("{id}")]
        public ActionResult<TaskResponseDto> UpdateTask(int id, UpdateTaskDto updateTask)
        {
            var updated = _taskService.UpdateTask(id, updateTask);

            if (updated is null)
                return NotFound();

            return Ok(updated);
        }

        [HttpPost]
        public ActionResult<TaskResponseDto> CreateTask(CreateTaskDto createTask)
        {
            var createdTask = _taskService.CreateTask(createTask);

            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id}, createdTask);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id) 
        {
            var deleted = _taskService.DeleteTask(id);

            if (!deleted) 
                return NotFound();

            return NoContent();
        }

    }
}
