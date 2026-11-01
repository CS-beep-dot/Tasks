namespace EmployeeManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var manager = new EmployeeManager();
            var input = new ConsoleInput();
            var actions = new EmployeeActions(manager, input);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("------ Employee Manager ------");
                Console.WriteLine("1. Add employee");
                Console.WriteLine("2. Update employee");
                Console.WriteLine("3. Remove employee");
                Console.WriteLine("4. Search by ID");
                Console.WriteLine("5. List all employees");
                Console.WriteLine("6. Filter by salary threshold");
                Console.WriteLine("7. Group by department");
                Console.WriteLine("8. Top three salaries");
                Console.WriteLine("0. Exit");

                int choice = input.ReadInt("Choose Option : ");
                if (choice == 0) return;

                try
                {
                    switch (choice)
                    {
                        case 1: actions.AddEmployee(); break;
                        case 2: actions.UpdateEmployee(); break;
                        case 3: actions.RemoveEmployee(); break;
                        case 4: actions.SearchById(); break;
                        case 5: actions.ListAll(); break;
                        case 6: actions.FilterBySalary(); break;
                        case 7: actions.PrintGroups(); break;
                        case 8: actions.TopThree(); break;
                        case 0: Console.WriteLine("Goodbye."); return;
                        default: Console.WriteLine("Invalid option. Enter a number from 0 to 8."); break;
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

        }
    }
}