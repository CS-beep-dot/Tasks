using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Tests;

public class ArchitectureTests
{
    [Fact]
    public void Domain_DoesNotReference_Application_Infrastructure_OrApi()
    {
        var domain = typeof(TaskItem).Assembly;

        var references = domain.GetReferencedAssemblies()
            .Select(a => a.Name)
            .ToList();

        string[] forbidden =
        [
            "TaskManagementApi.Application",
            "TaskManagementApi.Infrastructure",
            "TaskManagementApi",
            "Microsoft.EntityFrameworkCore",
            "Microsoft.AspNetCore"
        ];

        foreach (var name in forbidden)
        {
            Assert.DoesNotContain(references, r => r == name || r!.StartsWith(name + "."));
        }
    }
}
