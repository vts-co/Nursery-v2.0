using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.EmployeeReportTools
{
    public class EmployeeReportToolsDto
    {
        public System.Guid Id { get; set; }
        public Guid? EmployeeReportTypeId { get; set; }
        public string EmployeeReportTypeName { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}