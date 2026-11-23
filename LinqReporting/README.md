# LinqReporting

TASK-06 – Advanced LINQ Reporting. A small .NET 10 console app that builds employee reports with LINQ and compares `IEnumerable` with `IQueryable` (EF Core + SQLite).

## Features

- **Salary summary**: count, average and median salary
- **Department report**: employees grouped by department, with count, average and median
- **Hire-date ranking**: oldest hire first; employees with the same hire date are ordered by Id
- Reports return immutable `record` DTOs, not the `Employee` entity
- An empty input returns zeros or an empty list and never throws

## Project structure

```
LinqReporting/
├── Entity/      Employee
├── Dtos/        SalarySummaryDto, DepartmentReportDto, EmployeeRankDto
├── Reporting/   ReportingService
├── Data/        AppDbContext (EF Core), SeedData
└── Program.cs   example output + IQueryable/IEnumerable demo
LinqReporting.Tests/   xUnit tests
```

## How to run

```
dotnet run --project LinqReporting
dotnet test
```

## Deferred execution

A LINQ query does **not** run when you write it. It runs when you **use** the result: `foreach`, `ToList()`, `Count()`, `First()` and so on.

```csharp
var list = SeedData.GetEmployees();
var hrQuery = list.Where(e => e.Department == "HR");   // nothing runs yet
var hrSnapshot = hrQuery.ToList();                      // runs now: 3 employees

list.Add(new Employee(11, "Mari", "HR", ...));

hrQuery.Count();     // 4 - the query runs again and sees Mari
hrSnapshot.Count;    // 3 - ToList() saved the old result
```

What this means in practice:
- If the source changes before the query runs, the result includes the change.
- Using the same query twice runs it twice. With EF Core, that means two trips to the database.
- `ToList()` and `ToArray()` run the query **immediately** and save the result.

## IEnumerable vs IQueryable

|  | `IEnumerable<T>` | `IQueryable<T>` |
|---|---|---|
| Where it runs | in memory, in C# | in the database, as SQL |
| What EF loads | all rows | only the rows that match |
| Can use any C# method | yes | no: EF must be able to translate it to SQL |

Example from `Program.cs`, with the same filter on both:

```csharp
db.Employees.Where(e => e.Department == "IT");                 // IQueryable
db.Employees.AsEnumerable().Where(e => e.Department == "IT");  // IEnumerable
```

SQL sent by the **IQueryable** version. The filter is part of the SQL:
```sql
SELECT "e"."Id", "e"."Department", "e"."HireDate", "e"."Name", "e"."Salary"
FROM "Employees" AS "e"
WHERE "e"."Department" = 'IT'
```

SQL sent by the **IEnumerable** version. Every row is loaded, then C# filters them:
```sql
SELECT "e"."Id", "e"."Department", "e"."HireDate", "e"."Name", "e"."Salary"
FROM "Employees" AS "e"
```

Both return the same employees. The difference is where the work happens.

`ReportingService` takes `IEnumerable<Employee>`, so even if you pass it `db.Employees`, the grouping and median run in C#. That is on purpose: the median uses a C# helper method, which EF cannot translate to SQL.

## Example output

```
=== Reports ===
DepartmentReportDto { Department = Finance, Count = 3, Average = 7200, Median = 7100 }
DepartmentReportDto { Department = HR, Count = 3, Average = 3600, Median = 3300 }
DepartmentReportDto { Department = IT, Count = 4, Average = 5250, Median = 5000 }
EmployeeRankDto { Rank = 1, Id = 8, Name = Nino, Department = Finance, HireDate = 7/1/2017 12:00:00 AM }
EmployeeRankDto { Rank = 2, Id = 5, Name = Cato, Department = HR, HireDate = 11/5/2018 12:00:00 AM }
EmployeeRankDto { Rank = 3, Id = 1, Name = Ana, Department = IT, HireDate = 3/15/2019 12:00:00 AM }
EmployeeRankDto { Rank = 4, Id = 2, Name = Beka, Department = IT, HireDate = 6/1/2020 12:00:00 AM }
EmployeeRankDto { Rank = 5, Id = 9, Name = Sandro, Department = Finance, HireDate = 6/1/2020 12:00:00 AM }
EmployeeRankDto { Rank = 6, Id = 3, Name = Dato, Department = IT, HireDate = 9/10/2021 12:00:00 AM }
EmployeeRankDto { Rank = 7, Id = 6, Name = Giorgi, Department = HR, HireDate = 9/10/2021 12:00:00 AM }
EmployeeRankDto { Rank = 8, Id = 7, Name = Lia, Department = HR, HireDate = 2/14/2022 12:00:00 AM }
EmployeeRankDto { Rank = 9, Id = 4, Name = Eka, Department = IT, HireDate = 1/20/2023 12:00:00 AM }
EmployeeRankDto { Rank = 10, Id = 10, Name = Tamar, Department = Finance, HireDate = 4/8/2024 12:00:00 AM }
SalarySummaryDto { Count = 10, Average = 5340, Median = 5000 }

=== IQueryable ===
SELECT "e"."Id", "e"."Department", "e"."HireDate", "e"."Name", "e"."Salary"
FROM "Employees" AS "e"
WHERE "e"."Department" = 'IT'

=== IEnumerable ===
SELECT "e"."Id", "e"."Department", "e"."HireDate", "e"."Name", "e"."Salary"
FROM "Employees" AS "e"
Same result: True

=== Deferred execution ===
hrQuery.Count()  = 4  (runs again, sees Mari)
hrSnapshot.Count = 3  (ToList() saved the old result)
```

## Tests

15 xUnit tests cover:
- salary summary: empty list, odd and even counts, unsorted input
- department report: empty list, values for each department, sort order, `"IT"`/`"it"` grouped together
- hire-date ranking: empty list, oldest-first order, ties broken by Id
