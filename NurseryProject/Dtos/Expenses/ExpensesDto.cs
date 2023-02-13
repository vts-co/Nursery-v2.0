using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Expenses
{
    public class ExpensesDto
    {
        public Guid Id { get; set; }
        public Guid StudyPlaceId { get; set; }
        public string StudyPlaceName { get; set; }
        public Guid StudyYearId { get; set; }
        public string StudyYearName { get; set; }
        public Guid ExpenseTypePatentId { get; set; }
        public string ExpenseTypePatentName { get; set; }
        public Guid ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public Guid EmployeesReceiptId { get; set; }

        
    }
}