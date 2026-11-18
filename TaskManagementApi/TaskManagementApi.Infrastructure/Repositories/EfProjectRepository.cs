using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Application.Repositories;
using TaskManagementApi.Domain.Entities;
using TaskManagementApi.Infrastructure.Persistence;

namespace TaskManagementApi.Infrastructure.Repositories
{
    public class EfProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _db;

        public EfProjectRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        }

        public void Remove(Project project) => _db.Projects.Remove(project);

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
