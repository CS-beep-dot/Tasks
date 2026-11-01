using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement
{
    internal class ConsoleInput
    {
        public int ReadInt(string displayText)
        {
            while (true)
            {
                Console.Write(displayText);
                if (int.TryParse(Console.ReadLine(), out int value))
                {
                    return value;
                }
                Console.WriteLine("Enter Whole number");
            }
        }

        public DateTime ReadDateTime(string displayText)
        {
            while (true)
            {
                Console.Write(displayText);
                if (DateTime.TryParse(Console.ReadLine(), out DateTime hireDate))
                    return hireDate;
                Console.WriteLine("Enter with Correct format");
            }
        }

        public decimal ReadDecimal(string displayText)
        {
            while (true)
            {
                Console.Write(displayText);
                if (decimal.TryParse(Console.ReadLine(), out decimal value))
                    return value;
                Console.WriteLine("Enter a number like 1234.5 format");
            }
        }
    }
}
