using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.EmployeesAttendance
{
    public class EmployeesAttendanceDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeWorkShiftId { get; set; }
        public Guid StudyPlaceId { get; set; }
        public string StudyPlaceName { get; set; }
        public Guid StudyYearId { get; set; }
        public string StudyYearName { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Code { get; set; }

        public Guid WorkShiftId { get; set; }
        public string WorkShiftName { get; set; }
        public bool IsAttend { get; set; }
        public string Date { get; set; }
        public string All { get; set; }
        public string Attend { get; set; }

    }
}