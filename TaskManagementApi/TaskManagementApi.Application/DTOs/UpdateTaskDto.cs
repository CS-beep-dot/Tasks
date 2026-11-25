using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Application.DTOs
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public TaskItemStatus Status { get; set; }

        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }

        public int ProjectId { get; set; }   

    }
}
