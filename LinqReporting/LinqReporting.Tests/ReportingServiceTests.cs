using LinqReporting.Data;
using LinqReporting.Dtos;
using LinqReporting.Entity;
using LinqReporting.Reporting;

namespace LinqReporting.Tests;

public class ReportingServiceTests
{
    [Fact]
    public void GetSalarySummary_OfEmptyList_ReturnsZero()
    {
        var service = new ReportingService();

        var result = service.GetSalarySummary(new List<Employee>());
        Assert.Equal(0, result.Count);
        Assert.Equal(0m, result.Average);
    }

    [Fact]
    public void GetSalarySummary_OddCount_ReturnsMiddleSalaryAsMedian()
    {
        var service = new ReportingService();

        var employees = new List<Employee>
        {
            new Employee(1, "Ana", "IT", new DateTime(2020, 1, 1), 1000m),
            new Employee(2, "Beka", "IT", new DateTime(2021, 1, 1), 2000m),
            new Employee(3, "Cato", "HR", new DateTime(2022, 1, 1), 3000m)
        };

        var result = service.GetSalarySummary(employees);

        Assert.Equal(3, result.Count);
        Assert.Equal(2000m, result.Average);
        Assert.Equal(2000m, result.Median);
    }

    [Fact]
    public void GetMedianOfSalary_ReturnsZero()
    {
        var service = new ReportingService();
        var result = service.GetSalarySummary(new List<Employee>());

        Assert.Equal(0, result.Count);
        Assert.Equal(0m, result.Median);
    }

    [Fact]
    public void GetMedianOfSalary_EvenList()
    {
        var service = new ReportingService();
        var employees = new List<Employee>
        {
            new Employee(1, "Ana", "IT", new DateTime(2020, 1, 1), 1000m),
            new Employee(2, "Beka", "IT", new DateTime(2021, 1, 1), 2000m),
            new Employee(3, "Cato", "HR", new DateTime(2022, 1, 1), 3000m),
            new Employee(4, "George", "HR", new DateTime(2022, 1, 1), 5000m)
        };
        var result = service.GetSalarySummary(employees);

        Assert.Equal(4, result.Count);
        Assert.Equal(2500m, result.Median);
        Assert.Equal(2750m, result.Average);
    }


    [Fact]
    public void GetMedianOfSalary_UnsortedEvenList()
    {
        var service = new ReportingService();
        var employees = new List<Employee>
        {
            new Employee(1, "Ana", "IT", new DateTime(2020, 1, 1), 7000m),
            new Employee(2, "Beka", "IT", new DateTime(2021, 1, 1), 2000m),
            new Employee(3, "Cato", "HR", new DateTime(2022, 1, 1), 10000m),
            new Employee(4, "George", "HR", new DateTime(2022, 1, 1), 5000m)
        };
        var result = service.GetSalarySummary(employees);

        Assert.Equal(4, result.Count);
        Assert.Equal(6000m, result.Median);
        Assert.Equal(6000m, result.Average);
    }

    [Fact]
    public void GetMedianOfSalary_UnsortedOddList()
    {
        var service = new ReportingService();
        var employees = new List<Employee>
        {
            new Employee(1, "Ana", "IT", new DateTime(2020, 1, 1), 7000m),
            new Employee(2, "Beka", "IT", new DateTime(2021, 1, 1), 2000m),
            new Employee(4, "George", "HR", new DateTime(2022, 1, 1), 3000m)
        };
        var result = service.GetSalarySummary(employees);

        Assert.Equal(3, result.Count);
        Assert.Equal(3000m, result.Median);
        Assert.Equal(4000m, result.Average);
    }

    // ---------- GetDepartmentReport ----------

    [Fact]
    public void GetDepartmentReport_EmptyList_ReturnsEmpty()
    {
        var service = new ReportingService();

        var result = service.GetDepartmentReport(new List<Employee>());

        Assert.Empty(result);
    }

    [Theory]
    [InlineData("Finance", 3, 7200, 7100)]
    [InlineData("HR", 3, 3600, 3300)]
    [InlineData("IT", 4, 5250, 5000)]
    public void GetDepartmentReport_SeedData_ReturnsExpectedValues(string department, int count, int average, int median)
    {
        var service = new ReportingService();

        var result = service.GetDepartmentReport(SeedData.GetEmployees());

        var report = result.Single(r => r.Department == department);
        Assert.Equal(new DepartmentReportDto(department, count, average, median), report);
    }

    [Fact]
    public void GetDepartmentReport_SeedData_IsSortedByDepartmentName()
    {
        var service = new ReportingService();

        var result = service.GetDepartmentReport(SeedData.GetEmployees());

        Assert.Equal(new[] { "Finance", "HR", "IT" }, result.Select(r => r.Department));
    }

    [Fact]
    public void GetDepartmentReport_DifferentCasing_GroupsIntoOneDepartment()
    {
        var service = new ReportingService();
        var employees = new List<Employee>
        {
            new Employee(1, "Ana", "IT", new DateTime(2020, 1, 1), 1000m),
            new Employee(2, "Beka", "it", new DateTime(2021, 1, 1), 2000m),
            new Employee(3, "Cato", "It", new DateTime(2022, 1, 1), 3000m)
        };

        var result = service.GetDepartmentReport(employees);

        var report = Assert.Single(result);
        Assert.Equal(3, report.Count);
        Assert.Equal(2000m, report.Average);
    }

    // ---------- GetHireDateRanking ----------

    [Fact]
    public void GetHireDateRanking_EmptyList_ReturnsEmpty()
    {
        var service = new ReportingService();

        var result = service.GetHireDateRanking(new List<Employee>());

        Assert.Empty(result);
    }

    [Fact]
    public void GetHireDateRanking_SeedData_RanksOldestFirst()
    {
        var service = new ReportingService();

        var result = service.GetHireDateRanking(SeedData.GetEmployees());

        Assert.Equal(
            new[] { "Nino", "Cato", "Ana", "Beka", "Sandro", "Dato", "Giorgi", "Lia", "Eka", "Tamar" },
            result.Select(r => r.Name));
        Assert.Equal(Enumerable.Range(1, 10), result.Select(r => r.Rank));
    }

    [Fact]
    public void GetHireDateRanking_SameHireDate_LowerIdRanksFirst()
    {
        var service = new ReportingService();
        // Reversed input proves the order comes from sorting, not from input order.
        var employees = SeedData.GetEmployees().AsEnumerable().Reverse();

        var result = service.GetHireDateRanking(employees);

        var beka = result.Single(r => r.Name == "Beka");     // Id 2, 2020-06-01
        var sandro = result.Single(r => r.Name == "Sandro"); // Id 9, 2020-06-01
        Assert.Equal(4, beka.Rank);
        Assert.Equal(5, sandro.Rank);

        var dato = result.Single(r => r.Name == "Dato");     // Id 3, 2021-09-10
        var giorgi = result.Single(r => r.Name == "Giorgi"); // Id 6, 2021-09-10
        Assert.Equal(6, dato.Rank);
        Assert.Equal(7, giorgi.Rank);
    }
}
