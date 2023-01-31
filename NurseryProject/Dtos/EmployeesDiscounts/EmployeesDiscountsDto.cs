using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.EmployeesDiscounts
{
    public class EmployeesDiscountsDto
    {
        public Guid Id { get; set; }
        public Guid DiscountTypeId { get; set; }
        public string DiscountTypeName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public string Value { get; set; }
        public string Reason { get; set; }
    }
}