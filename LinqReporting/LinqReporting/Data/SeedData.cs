using LinqReporting.Entity;

namespace LinqReporting.Data
{
    /// Fixed dataset shared by tests, Program.cs and the EF demo.
    public static class SeedData
    {
        public static List<Employee> GetEmployees() => new()
        {
            new Employee(1,  "Ana",    "IT",      new DateTime(2019, 3, 15), 4000m),
            new Employee(2,  "Beka",   "IT",      new DateTime(2020, 6, 1),  5500m),
            new Employee(3,  "Dato",   "IT",      new DateTime(2021, 9, 10), 7000m),
            new Employee(4,  "Eka",    "IT",      new DateTime(2023, 1, 20), 4500m),
            new Employee(5,  "Cato",   "HR",      new DateTime(2018, 11, 5), 3000m),
            new Employee(6,  "Giorgi", "HR",      new DateTime(2021, 9, 10), 3300m),
            new Employee(7,  "Lia",    "HR",      new DateTime(2022, 2, 14), 4500m),
            new Employee(8,  "Nino",   "Finance", new DateTime(2017, 7, 1),  6500m),
            new Employee(9,  "Sandro", "Finance", new DateTime(2020, 6, 1),  8000m),
            new Employee(10, "Tamar",  "Finance", new DateTime(2024, 4, 8),  7100m),
        };
    }
}
