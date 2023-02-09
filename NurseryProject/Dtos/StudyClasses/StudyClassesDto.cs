using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.StudyClasses
{
    public class StudyClassesDto
    {
        public Guid Id { get; set; }
        public Guid StudyYearId { get; set; }
        public string StudyYearName { get; set; }
        public int DisplayOrder { get; set; }

        public string Name { get; set; }
        public string Notes { get; set; }
    }
}