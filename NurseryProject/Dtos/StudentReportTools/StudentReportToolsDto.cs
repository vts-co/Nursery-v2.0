using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.StudentReportTools
{
    public class StudentReportToolsDto
    {
        public System.Guid Id { get; set; }
        public Guid? StudentReportTypeId { get; set; }
        public string StudentReportTypeName { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}