using System;
using System.Collections.Generic;
using System.Text;

namespace LinqReporting.Dtos
{
    public record EmployeeRankDto(int Rank, int Id, string Name, string Department, DateTime HireDate);
}
