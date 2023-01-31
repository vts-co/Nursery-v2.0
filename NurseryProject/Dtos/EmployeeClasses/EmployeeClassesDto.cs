using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.EmployeeClasses
{
    public class EmployeeClassesDto
    {
        public Guid Id { get; set; }
        public Guid StudyPlaceId { get; set; }
        public string StudyPlaceName { get; set; }
        public Guid StudyTypeId { get; set; }
        public string StudyTypeName { get; set; }
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public Guid StudyYearId { get; set; }
        public string StudyYearName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjecName { get; set; }
     
    }
}