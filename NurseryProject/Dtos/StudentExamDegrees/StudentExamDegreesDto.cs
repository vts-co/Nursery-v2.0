using NurseryProject.Dtos.Exams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.StudentExamDegrees
{
    public class StudentExamDegreesDto
    {
        public Guid Id { get; set; }
        public Guid StudyPlaceId { get; set; }
        public string StudyPlaceName { get; set; }
        public Guid StudyYearId { get; set; }
        public string StudyYearName { get; set; }
        public Guid StudyTypeId { get; set; }
        public string StudyTypeName { get; set; }
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public Guid ExamId { get; set; }
        public string ExamName { get; set; }
        public string ExamDegree { get; set; }

        public bool IsOneQuestion { get; set; }
        public string Date { get; set; }
        public string Count { get; set; }
        public string Code { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentPhone { get; set; }
        public string TotalDegree { get; set; }


        public List<StudentExamDegreesDetailsDto> Students { get; set; }

    }
    public class StudentExamDegreesDetailsDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Count { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentPhone { get; set; }
        public Guid ExamDegreeId { get; set; }
        public string TotalDegree { get; set; }
        public string Date { get; set; }

    }
}