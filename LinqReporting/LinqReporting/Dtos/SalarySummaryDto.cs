using System;
using System.Collections.Generic;
using System.Text;

namespace LinqReporting.Dtos
{
    public record SalarySummaryDto(int Count, decimal Average, decimal Median);
}
