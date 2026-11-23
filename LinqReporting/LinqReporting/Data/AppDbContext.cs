using System;
using System.Collections.Generic;
using System.Text;
using LinqReporting.Entity;
using Microsoft.EntityFrameworkCore;

namespace LinqReporting.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees => Set<Employee>();
    }
}
