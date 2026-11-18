using System;
using System.Collections.Generic;
using System.Text;
using TaskManagementApi.Application.Repositories;

namespace TaskManagementApi.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repository;

        public ProjectService(IProjectRepository projectRepository) { 
            _repository = projectRepository;
        }

        public async Task<bool> DeleteProject(int id)
        {
            var project = await _repository.GetByIdAsync(id);

            if (project == null)
            {
                return false;
            }

            _repository.Remove(project);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
