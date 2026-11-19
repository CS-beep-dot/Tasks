using System;
using System.Collections.Generic;
using System.Text;
using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Application.Services
{
    public interface IProjectService
    {
        Task<bool> DeleteProjectAsync(int id);
    }
}
