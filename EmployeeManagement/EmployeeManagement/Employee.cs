using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement
{
    internal class Employee
    {
        public int Id { get; }
        public string Name { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }

        public Employee(int id, string name, string department, DateTime hireDate,  decimal salary)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be a positive number.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be blank.");
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Department cannot be blank.");
            if (salary < 0)
                throw new ArgumentException("Salary cannot be negative.");

            Id = id;
            Name = name;
            Department = department;
            HireDate = hireDate;
            Salary = salary;
        }

        public override string ToString() =>
            $"ID:         {Id}{Environment.NewLine}" +
            $"Name:       {Name}{Environment.NewLine}" +
            $"Department: {Department}{Environment.NewLine}" +
            $"Hire date:  {HireDate:yyyy-MM-dd}{Environment.NewLine}" +
            $"Salary:     {Salary:N2}";
    }
}
