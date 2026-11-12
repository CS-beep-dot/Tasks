using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.DTOs;
using TaskManagementApi.Exceptions;
using TaskManagementApi.Domain.Entities;
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
        public async Task<ActionResult<PagedResult<TaskResponseDto>>> GetAllTasks(
            [FromQuery] TaskItemStatus? status,
            [FromQuery] int? assigneeId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            return Ok(await _taskService.GetTasks(status, assigneeId, page, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponseDto>> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskById(id);

            if (task is null)
                return NotFound();

            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskResponseDto>> UpdateTask(int id, UpdateTaskDto updateTask)
        {
            try
            {
                var updated = await _taskService.UpdateTask(id, updateTask);

                if (updated is null)
                    return NotFound();

                return Ok(updated);
            }
            catch (InvalidReferenceException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> CreateTask(CreateTaskDto createTask)
        {
            try
            {
                var createdTask = await _taskService.CreateTask(createTask);

                return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
            }
            catch (InvalidReferenceException ex)
            {
                return BadRequest(new { error = ex.Message});
            }
           
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id) 
        {
            var deleted = await _taskService.DeleteTask(id);

            if (!deleted) 
                return NotFound();

            return NoContent();
        }

    }
}
