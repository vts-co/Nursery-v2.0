using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Subjects
{
    public class SubjectsDto
    {
        public Guid Id { get; set; }
        public Guid StudyTypeId { get; set; }
        public string StudyTypeName { get; set; }
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
   
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}