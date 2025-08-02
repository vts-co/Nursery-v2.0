using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Revenues
{
    public class RevenuesDto
    {
        public Guid Id { get; set; }
        public Guid StudyPlaceId { get; set; }
        public string StudyPlaceName { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public Guid StudyYearId { get; set; }
        public string StudyYearName { get; set; }
        public Guid RevenueTypeParentId { get; set; }
        public string RevenueTypeParentName { get; set; }
        public Guid RevenueTypeId { get; set; }
        public string RevenueTypeName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PaperNumber1 { get; set; }
        public string PaperNumber2 { get; set; }
        public Guid SubscriptionMethodId { get; set; }
        
        public double? Value { get; set; }
        public DateTime Date { get; set; }

        public string Notes { get; set; }
    }
}