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


        public SalarySummary GetSalarySummary(IEnumerable<Employee> employees)
        {
            var salaries = employees.Select(e => e.Salary).ToList();

            if (salaries.Count == 0)
                return new SalarySummary(0, 0m, 0m);

            return new SalarySummary(salaries.Count, salaries.Average(), GetSalaryMedian(salaries));
        }

        public IReadOnlyList<DepartmentReport> GetDepartmentReport(IEnumerable<Employee> employees)
        {

            var result = employees.GroupBy(e => e.Department, StringComparer.OrdinalIgnoreCase)
                .Select(g => new DepartmentReport(
                    g.Key,
                    g.Count(),
                    g.Average(e => e.Salary),
                    GetSalaryMedian(g.Select(e => e.Salary).ToList())))
                .OrderBy(r => r.Department)
                .ToList();

            return result;
        }
    }
}
