using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement
{
    internal class EmployeeManager
    {
        private List<Employee> employees = new List<Employee>();

        public bool AddEmployee(Employee _employee)
        {
            var idAlreadyExists = employees.Any(e => e.Id == _employee.Id);

            if (idAlreadyExists)
            {
                return false;
            }

            employees.Add(_employee);
            return true;
        }

        public Employee? SearchById(int id)
        {
            return employees.FirstOrDefault(e => e.Id == id);
        }

        public bool UpdateEmployee(int id, string name, string department, DateTime hireDate, decimal salary)
        {
            int index = employees.FindIndex(e => e.Id == id);
            if (index < 0) return false;

            employees[index] = new Employee(id, name, department, hireDate, salary);
            return true;
        }

        public bool RemoveEmployee(int id)
        {
            return employees.RemoveAll(e => e.Id == id) > 0;
        }

        public IList<Employee> GetAll() {
            return employees.OrderBy(e => e.Id).ToList();
        }

        public IList<Employee> GetTopThree()
        {
            return employees
                .OrderByDescending(e => e.Salary)
                .Take(3)
                .ToList();
        }

        public IList<Employee> GetSalaryHigherThan(decimal threshold)
        {
            return employees
                .Where(e => e.Salary > threshold)
                .ToList();
        }

        public Dictionary<string, List<Employee>> GroupByDepartments()
        {
            return employees
                .GroupBy(e => e.Department, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key ,g => g.ToList(), StringComparer.OrdinalIgnoreCase);
        }
       
    }
}
