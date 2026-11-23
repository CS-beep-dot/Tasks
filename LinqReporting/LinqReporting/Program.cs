using LinqReporting.Data;
using LinqReporting.Entity;
using LinqReporting.Reporting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
using var db = new AppDbContext(options);
db.Database.EnsureCreated();
db.Employees.AddRange(SeedData.GetEmployees());
db.SaveChanges();

var service = new ReportingService();
var employees = SeedData.GetEmployees();

// 1. Reports
Console.WriteLine("=== Reports ===");
foreach (var d in service.GetDepartmentReport(employees))
    Console.WriteLine(d);
foreach (var r in service.GetHireDateRanking(employees))
    Console.WriteLine(r);
Console.WriteLine(service.GetSalarySummary(employees));

// 2. IQueryable: the filter is translated to SQL and runs in the database
Console.WriteLine("\n=== IQueryable ===");
IQueryable<Employee> queryable = db.Employees.Where(e => e.Department == "IT");
Console.WriteLine(queryable.ToQueryString()); 

//IEnumerable: all rows are loaded, then the filter runs in C#
Console.WriteLine("\n=== IEnumerable ===");
IEnumerable<Employee> enumerable = db.Employees.AsEnumerable().Where(e => e.Department == "IT");
Console.WriteLine(db.Employees.ToQueryString());
Console.WriteLine("Same result: " + (queryable.Count() == enumerable.Count()));

//query runs when it is used
Console.WriteLine("\n=== Deferred execution ===");
var list = SeedData.GetEmployees();
var hrQuery = list.Where(e => e.Department == "HR");
var hrSnapshot = hrQuery.ToList();

list.Add(new Employee(11, "Mari", "HR", new DateTime(2025, 1, 6), 3900m));

Console.WriteLine($"hrQuery.Count()  = {hrQuery.Count()}  (runs again, sees Mari)");
Console.WriteLine($"hrSnapshot.Count = {hrSnapshot.Count}  (ToList() saved the old result)");