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
}
