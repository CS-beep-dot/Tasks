using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.DTOs;
using TaskManagementApi.Application.Exceptions;
using TaskManagementApi.Domain.Entities;
using TaskManagementApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;

namespace TaskManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IValidator<UpdateTaskDto> _updateValidator;
        private readonly IValidator<CreateTaskDto> _createValidator;

        public TasksController(
            ITaskService taskService,
            IAuthorizationService authorizationService,
            IValidator<UpdateTaskDto> updateValidator,
            IValidator<CreateTaskDto> createValidator
            )
        {
            _taskService = taskService;
            _authorizationService = authorizationService;
            _updateValidator = updateValidator;
            _createValidator = createValidator;
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
            var result = await _updateValidator.ValidateAsync(updateTask);
            if (!result.IsValid)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
                return ValidationProblem(ModelState);
            }

            var existing = await _taskService.GetTaskById(id);
            if (existing is null) return NotFound();

            var auth = await _authorizationService.AuthorizeAsync(User, existing, "CanUpdateTasks");
            if (!auth.Succeeded) return Forbid();

            var updated = await _taskService.UpdateTask(id, updateTask);

            if (updated is null)
                return NotFound();
            return Ok(updated);
        }

        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> CreateTask(CreateTaskDto createTask)
        {            
            var result = await _createValidator.ValidateAsync(createTask);
            if (!result.IsValid)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
                return ValidationProblem(ModelState);
            }

            var createdTask = await _taskService.CreateTask(createTask);

            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDeleteTasks")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _taskService.DeleteTask(id);

            if (!deleted) 
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/assignee")]
        [Authorize(Policy = "CanAssignTasks")]
        public async Task<ActionResult> UpdateAssignee(int id, AssignTaskDto dto)
        {

            var updated = await _taskService.AssignTask(id, dto.AssigneeId);
        
            if (updated is null)
                return NotFound();

            return Ok(updated);
        }

    }
}
