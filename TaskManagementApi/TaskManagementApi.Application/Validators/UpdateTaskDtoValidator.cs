using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagementApi.Application.DTOs;

namespace TaskManagementApi.Application.Validators
{
    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {

        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                 .NotEmpty().WithMessage("Title is required")
                 .MaximumLength(150).WithMessage("Title must be max of 150 character");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(1).WithMessage("Description must be at least")
                .MaximumLength(800).WithMessage("Description must be max of 800 character");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Enter correct Status (Pending, InProgress, Completed)");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Enter Valid Priority (Low, Medium, High)");

            RuleFor(x => x.DueDate).NotNull().Must(d => d > DateTime.UtcNow);

            RuleFor(x => x.ProjectId).InclusiveBetween(1, int.MaxValue);
        }

    }
}
