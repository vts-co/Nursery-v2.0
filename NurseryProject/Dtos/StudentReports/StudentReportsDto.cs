using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.StudentReports
{
    public class StudentReportsDto
    {
        public System.Guid Id { get; set; }
        public Guid? StudentReportTypeId { get; set; }
        public string StudentReportTypeName { get; set; }
        public Guid? StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime ReportDateFrom { get; set; }
        public DateTime ReportDateTo { get; set; }
        public List<StudentReportItemsDto> StudentReportItems { get; set; }
        public string Notes { get; set; }
    }
    public class StudentReportItemsDto
    {
        public System.Guid Id { get; set; }
        public System.Guid StudentReportId { get; set; }
        public System.Guid StudentReportToolId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Notes { get; set; }
    }
}