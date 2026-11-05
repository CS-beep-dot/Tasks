using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models
{
    [Index(nameof(ProjectId))]
    [Index(nameof(Status))]
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(800)]
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime DueDate { get; set; }


        public int ProjectId { get; set; }             // required
        public Project Project { get; set; } = null!;

        public int? AssigneeId { get; set; }         //optionals
        public User? Assignee { get; set; }
    }
}
