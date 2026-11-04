using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    }
}
