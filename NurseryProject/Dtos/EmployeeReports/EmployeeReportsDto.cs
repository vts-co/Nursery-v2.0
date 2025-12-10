using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.EmployeeReports
{
    public class EmployeeReportsDto
    {
        public System.Guid Id { get; set; }
        public Guid? EmployeeReportTypeId { get; set; }
        public string EmployeeReportTypeName { get; set; }
        public Guid? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime ReportDateFrom { get; set; }
        public DateTime ReportDateTo { get; set; }
        public List<EmployeeReportItemsDto> EmployeeReportItems { get; set; }
        public string Notes { get; set; }
    }
    public class EmployeeReportItemsDto
    {
        public System.Guid Id { get; set; }
        public System.Guid EmployeeReportId { get; set; }
        public System.Guid EmployeeReportToolId { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
        public string Notes { get; set; }
    }
}