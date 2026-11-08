using System;
using System.Collections.Generic;
using System.Text;

namespace LinqReporting.Reporting
{
    public record DepartmentReport(string Department, int Count, decimal Average, decimal Median);
}
