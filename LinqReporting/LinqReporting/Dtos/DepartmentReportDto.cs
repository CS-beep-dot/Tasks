using System;
using System.Collections.Generic;
using System.Text;

namespace LinqReporting.Dtos
{
    public record DepartmentReportDto(string Department, int Count, decimal Average, decimal Median);
}
