using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement
{
    internal class EmployeeActions
    {
        private readonly EmployeeManager _manager;
        private readonly ConsoleInput _input;

        public EmployeeActions(EmployeeManager manager, ConsoleInput input)
        {
            _manager = manager;
            _input = input;
        }

        public void PrintGroups()
        {
            var groups = _manager.GroupByDepartments();

            if (groups.Count == 0)
            {
                Console.WriteLine("No groups found");
                return;
            }

            foreach (var group in groups)
            {
                Console.WriteLine($"---- {group.Key}  : {group.Value.Count} ");

                foreach (var employee in group.Value) 
                {
                    Console.WriteLine(employee);
                }
            }
        }

        public void FilterBySalary()
        {
            decimal threshold = _input.ReadDecimal("Enter minimum salary : ");
            PrintEmployees(_manager.GetSalaryHigherThan(threshold));

        }

        public void ListAll()
        {
            PrintEmployees(_manager.GetAll());
        }

        public void TopThree()
        {
            PrintEmployees(_manager.GetTopThree());
        }

        public void PrintEmployees(IList<Employee> employees)
        {
            if (employees.Count == 0)
            {
                Console.WriteLine("no employee found");
                return;
            }

            foreach (var employee in employees)
                Console.WriteLine(employee);
        }

        public void SearchById()
        {
            int employeeId = _input.ReadInt("Enter ID to search : ");

            Employee? employee = _manager.SearchById(employeeId);

            if (employee == null)
            {
                Console.WriteLine($"No Employee with given ID {employeeId} exists");
            }
            else
                Console.WriteLine(employee);
        }

        public void RemoveEmployee()
        {
            int id = _input.ReadInt("Enter ID of employee to Delete : ");

            if (_manager.RemoveEmployee(id))
                Console.WriteLine("Employee removed seuccesfully");
            else
                Console.WriteLine($"No employee with id : {id}");

        }

        public void UpdateEmployee()
        {
            int id = _input.ReadInt("Enter ID of employee to update : ");

            if (_manager.SearchById(id) == null)
            {
                Console.WriteLine($"No Employee with given ID {id} exists");
                return;
            }

            Console.Write("Enter new name : ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter new department : ");
            string department = Console.ReadLine() ?? "";

            DateTime hireDate = _input.ReadDateTime("Enter new hire date (yyyy-MM-dd) : ");
            decimal salary = _input.ReadDecimal("Enter new salary in decimal ex 1234.5 : ");

            _manager.UpdateEmployee(id, name, department, hireDate, salary);
            Console.WriteLine("Employee updated successfully");
        }

        public void AddEmployee()
        {
            int id = _input.ReadInt("Enter ID: ");

            Console.Write("Enter name : ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter department : ");
            string department = Console.ReadLine() ?? "";

            DateTime hireDate = _input.ReadDateTime("Enter hire date (yyyy-MM-DD) : ");

            decimal salary = _input.ReadDecimal("Enter salary in decimal ex 1234.5 : ");

            if (_manager.AddEmployee(new Employee(id, name, department, hireDate, salary)))
                Console.WriteLine("Employee added successfully");
            else
                Console.WriteLine("Employee with this id already exists");
        }


    }
}
