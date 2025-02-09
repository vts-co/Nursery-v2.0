using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Revenues
{
    public class FinancialReportDto
    {

        public double? Expences { get; set; }
        public double? Income { get; set; }
        public double Salaries { get; set; }
        public double Subscriptions { get; set; }
        public double? Safy { get; set; }
    }
}