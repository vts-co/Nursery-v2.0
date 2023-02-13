using NurseryProject.Dtos.EmployeesDiscounts;
using NurseryProject.Dtos.EmployeesIncreases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.EmployeesReceipt
{
    public class EmployeesReceiptDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Month { get; set; }

        public string Department { get; set; }
        public string Jop { get; set; }
        public string WorkDayCost { get; set; }
        public int EmployeesAttended { get; set; }
        public int EmployeesNoAttended { get; set; }

        public string TotalCost { get; set; }
        public List<EmployeesDiscounts> EmployeesDiscount { get; set; }
        public List<EmployeesIncreases> EmployeesIncreases { get; set; }
        public string TotalDiscountCost { get; set; }
        public string TotalIncreasesCost { get; set; }
        public string FinalTotalCost { get; set; }
        public string Reciept { get; set; }
        public string Paid { get; set; }
        public string Date { get; set; }
        public string StudyPlaceName { get; set; }
        public string StudyYearName { get; set; }
    }
    public class EmployeesDiscounts
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
    public class EmployeesIncreases
    {
        public Guid Id { get; set; }
        public Guid IncreaseTypeId { get; set; }
        public string IncreaseTypeName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public string Value { get; set; }
        public string Reason { get; set; }
    }
}