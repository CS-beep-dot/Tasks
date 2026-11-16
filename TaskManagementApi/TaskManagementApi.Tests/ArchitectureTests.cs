using System.Reflection;
using TaskManagementApi.Application.Services;
using TaskManagementApi.Domain.Entities;

namespace TaskManagementApi.Tests;

public class ArchitectureTests
{
    private const string Application = "TaskManagementApi.Application";
    private const string Infrastructure = "TaskManagementApi.Infrastructure";
    private const string Api = "TaskManagementApi";

    [Fact]
    public void Domain_DoesNotReference_Application_Infrastructure_OrApi()
    {
        AssertDoesNotReference(
            typeof(TaskItem).Assembly,
            projects: [Application, Infrastructure, Api],
            frameworks: ["Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore"]);
    }

    [Fact]
    public void Application_DoesNotReference_Infrastructure_OrApi()
    {
        AssertDoesNotReference(
            typeof(TaskService).Assembly,
            projects: [Infrastructure, Api],
            frameworks: ["Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore"]);
    }

    // projects: exact assembly names. frameworks: the name or any sub-assembly (e.g. Microsoft.EntityFrameworkCore.SqlServer).
    private static void AssertDoesNotReference(Assembly assembly, string[] projects, string[] frameworks)
    {
        var references = assembly.GetReferencedAssemblies()
            .Select(a => a.Name!)
            .ToList();

        foreach (var project in projects)
        {
            Assert.DoesNotContain(project, references);
        }

        foreach (var framework in frameworks)
        {
            Assert.DoesNotContain(references, r => r == framework || r.StartsWith(framework + "."));
        }
    }
}
