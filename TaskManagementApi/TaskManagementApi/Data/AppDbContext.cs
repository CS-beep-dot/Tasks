using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


    }
}
