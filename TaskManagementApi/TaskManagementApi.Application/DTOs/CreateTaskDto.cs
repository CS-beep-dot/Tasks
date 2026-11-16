using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.DTOs
{
    public class CreateTaskDto
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
        // Nullable + [Required]: a plain DateTime would silently become 0001-01-01 when missing
        [Required]
        public DateTime? DueDate { get; set; }

        [Range(1, int.MaxValue)]
        public int ProjectId { get; set; }  

        public int? AssigneeId { get; set; }

    }
}
