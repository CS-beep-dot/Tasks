using System;
using System.Collections.Generic;
using System.Text;

namespace LinqReporting.Entity
{
    public class Employee
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }

        public Employee(int id, string name, string department, DateTime hireDate, decimal salary)
        {
            Id = id;
            Name = name;
            Department = department;
            HireDate = hireDate;
            Salary = salary;
        }
    }
}
