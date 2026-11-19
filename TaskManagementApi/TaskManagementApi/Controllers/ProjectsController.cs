using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.Services;

namespace TaskManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDeleteProjects")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            return await _projectService.DeleteProjectAsync(id) ? NoContent() : NotFound();
        }
        
    }
}
