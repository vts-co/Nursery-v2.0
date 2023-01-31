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
        public Guid StudyYearId { get; set; }
        public string StudyYearName { get; set; }
        public Guid RevenueTypeId { get; set; }
        public string RevenueTypeName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }

        public string Notes { get; set; }
    }
}