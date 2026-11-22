using Microsoft.AspNetCore.Authorization;
using TaskManagementApi.Application.DTOs;

namespace TaskManagementApi.Authorization
{
    //Managers and Admins can update any task, everyone else only tasks assigned to them.
    public class CanUpdateTaskHandler : AuthorizationHandler<CanUpdateTaskRequirement, TaskResponseDto>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanUpdateTaskRequirement requirement,
            TaskResponseDto task)
        {
            if (context.User.IsInRole("Manager") || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // "sub" holds the user id
            var userId = context.User.FindFirst("sub")?.Value;

            if (userId != null && task.AssigneeId?.ToString() == userId)
            {
                context.Succeed(requirement);
            }

            //requirement fails, so the request gets 403.
            return Task.CompletedTask;
        }
    }
}
