using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Employees
{
    public class EmployeesDto
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Guid JopId { get; set; }
        public string JopName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string BirthDate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Qualification { get; set; }
        public string Image { get; set; }

        public string JoiningDate { get; set; }
        public int GenderId { get; set; }
        public string GenderName { get; set; }
        public int MaritalStateId { get; set; }
        public string MaritalStateName { get; set; }
        public string WorkDayCost { get; set; }
        public string Notes { get; set; }
    }
}