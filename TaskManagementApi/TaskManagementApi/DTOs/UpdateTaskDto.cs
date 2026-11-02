using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Models;

namespace TaskManagementApi.DTOs
{
    public class UpdateTaskDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(800)]
        public string Description { get; set; } = string.Empty;

        [EnumDataType(typeof(TaskItemStatus))]
        public TaskItemStatus Status { get; set; }

        [EnumDataType(typeof(TaskPriority))]
        public TaskPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
    }
}
