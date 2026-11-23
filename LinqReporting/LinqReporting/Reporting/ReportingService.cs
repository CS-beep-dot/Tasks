using LinqReporting.Dtos;
using LinqReporting.Entity;

namespace LinqReporting.Reporting
{
    public class ReportingService
    {
        private static decimal GetSalaryMedian(List<decimal> salaries)
        {
            if (salaries.Count == 0)
                return 0m;

            var result = salaries.OrderBy(s => s).ToList();
            var middleIndex = salaries.Count / 2;
            if (salaries.Count % 2 == 0)
            {
                return (result[middleIndex - 1] + result[middleIndex]) / 2m;
            }

            return result[middleIndex];
        }


        public SalarySummaryDto GetSalarySummary(IEnumerable<Employee> employees)
        {
            var salaries = employees.Select(e => e.Salary).ToList();

            if (salaries.Count == 0)
                return new SalarySummaryDto(0, 0m, 0m);

            return new SalarySummaryDto(salaries.Count, salaries.Average(), GetSalaryMedian(salaries));
        }

        public IReadOnlyList<DepartmentReportDto> GetDepartmentReport(IEnumerable<Employee> employees)
        {

            var result = employees.GroupBy(e => e.Department, StringComparer.OrdinalIgnoreCase)
                .Select(g => new DepartmentReportDto(
                    g.Key,
                    g.Count(),
                    g.Average(e => e.Salary),
                    GetSalaryMedian(g.Select(e => e.Salary).ToList())))
                .OrderBy(r => r.Department, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return result;
        }

        public IReadOnlyList<EmployeeRankDto> GetHireDateRanking(IEnumerable<Employee> employees)
        {

            var result = employees
                .OrderBy(e => e.HireDate)
                .ThenBy(e => e.Id)
                .Select((e, index) => new EmployeeRankDto(
                    index + 1,
                    e.Id,
                    e.Name,
                    e.Department,
                    e.HireDate
                )).ToList();
            return result;
        }
    }
}
