using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.EmployeesTransferAllowance
{
    public class EmployeesTransferAllowanceDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public string Value { get; set; }
    }
}