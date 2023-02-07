using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.StudentsClassesTransfer
{
    public class StudentsClassesTransferDto
    {
        public Guid Id { get; set; }

        public Guid StudentClassId { get; set; }

        public Guid StudyYearId { get; set; }
        public string StudyYearName { get; set; }

        public Guid StudyPlaceId { get; set; }
        public string StudyPlaceName { get; set; }

        public Guid StudyTypeId { get; set; }
        public string StudyTypeName { get; set; }

        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
       
        public Guid ClassFromId { get; set; }
        public string ClassFromName { get; set; }

        public Guid ClassToId { get; set; }
        public string ClassToName { get; set; }

        public Guid StudentId { get; set; }
        public string Code { get; set; }

        public string StudentName { get; set; }

        public string Date { get; set; }

        public string Notes { get; set; }
    }
}