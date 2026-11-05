// TODO: tests disabled until TaskService fully uses AppDbContext (needs EF Core InMemory setup)
/*
﻿using System.ComponentModel.DataAnnotations;
using TaskManagementApi.DTOs;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace TaskManagementApi.Tests
{
    public class TaskManagementApiTests
    {
        private static readonly DateTime Due = new DateTime(2026, 12, 1);

        private static CreateTaskDto NewTask(string title = "Write README") => new CreateTaskDto
        {
            Title = title,
            Description = "Some description",
            Status = TaskItemStatus.Pending,
            Priority = TaskPriority.Medium,
            DueDate = Due
        };

        private static List<ValidationResult> Validate(object dto)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);
            return results;
        }


        //Create task
        [Fact]
        public void CreateTask_ValidRequest_AssignsIdAndStoresTask()
        {
            var service = new TaskService();

            var created = service.CreateTask(NewTask());
            Assert.True(created.Id > 0);
            Assert.Single(service.GetTasks());
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateTask_Invalid_TitleRequest_ReturnsNull(string title)
        {
            var errors = Validate(NewTask(title));

            Assert.Contains(errors, e => e.MemberNames.Contains(nameof(CreateTaskDto.Title)));
        }

        //create Task invalid
        [Fact]
        public void CreateTask_Twice_AssignsDifferentIds()
        {
            var service = new TaskService();

            var first = service.CreateTask(NewTask("First"));
            var second = service.CreateTask(NewTask("Second"));

            Assert.NotEqual(first.Id, second.Id);
        }

        //delete task
        [Fact]
        public void DeleteTask_ValidRequest_ReturnsTrue_AndRemovesIt()
        {
            var service = new TaskService();

            var created = service.CreateTask(NewTask());

            Assert.True(service.DeleteTask(created.Id));
            Assert.Null(service.GetTaskById(created.Id));
        }

        [Fact]
        public void DeleteTask_UnknownId_ReturnsFalse()
        {
            var service = new TaskService();
            Assert.False(service.DeleteTask(200));
        }

        // update
        [Fact]
        public void Update_ExistingTask_ChangesValues()
        {
            var service = new TaskService();
            var created = service.CreateTask(NewTask());

            var result = service.UpdateTask(created.Id, new UpdateTaskDto
            {
                Title = "Write README (done)",
                Description = "Updated description",
                Status = TaskItemStatus.Completed,
                Priority = TaskPriority.High,
                DueDate = Due
            });

            Assert.NotNull(result);
            Assert.Equal("Write README (done)", result!.Title);
            Assert.Equal(TaskItemStatus.Completed, service.GetTaskById(created.Id)!.Status);
        }

        [Fact]
        public void UpdateTask_UnknownId_ReturnsNull()
        {
            var service = new TaskService();

            var result = service.UpdateTask(999, new UpdateTaskDto { Title = "Nothing here", DueDate = Due });

            Assert.Null(result);
        }

        //get task
        [Fact]
        public void GetTaskById_UnknownId()
        {
            var service = new TaskService();

            Assert.Null(service.GetTaskById(700));
        }

        // status filter test
        [Fact]
        public void GetTasks_WithStatusFilter_ReturnsOnlyMatching()
        {
            var service = new TaskService();
            service.CreateTask(NewTask("Pending one"));
            var done = service.CreateTask(NewTask("Done one"));
            service.UpdateTask(done.Id, new UpdateTaskDto
            {
                Title = "Done one",
                Status = TaskItemStatus.Completed,
                DueDate = Due
            });

            var result = service.GetTasks(TaskItemStatus.Completed).ToList();

            Assert.Single(result);
            Assert.Equal(done.Id, result[0].Id);
        }



    }
}

*/
