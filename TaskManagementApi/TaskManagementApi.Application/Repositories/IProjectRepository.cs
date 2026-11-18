using System;
using System.Collections.Generic;
using System.Text;
using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Application.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(int id);
        void Remove(Project project);
        Task SaveChangesAsync();
    }
}
