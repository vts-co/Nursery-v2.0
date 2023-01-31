using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.BuildingSupervisors
{
    public class BuildingSupervisorsDto
    {
        public Guid Id { get; set; }
        public Guid StudyPlaceId { get; set; }
        public string StudyPlaceName { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Notes { get; set; }
    }
}