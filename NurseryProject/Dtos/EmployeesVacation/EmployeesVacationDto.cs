using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.EmployeesVacation
{
    public class EmployeesVacationDto
    {
        public Guid Id { get; set; }
        public Guid VacationTypeId { get; set; }
        public string VacationTypeName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Reason { get; set; }

    }
}