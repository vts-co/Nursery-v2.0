using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Levels
{
    public class LevelsDto
    {
        public Guid Id { get; set; }
        public Guid StudyTypeId { get; set; }
        public string StudyTypeName { get; set; }
        public string FromAge { get; set; }
        public string ToAge { get; set; }
        public int DisplayOrder { get; set; }

        
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}