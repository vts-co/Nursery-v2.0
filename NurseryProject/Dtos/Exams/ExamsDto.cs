using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Exams
{
    public class ExamsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid StudyTypeId { get; set; }
        public string StudyTypeName { get; set; }
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; }
        public Guid ExamTypeId { get; set; }
        public string ExamTypeName { get; set; }
        public bool IsOneQuestion { get; set; }
        public string TotalDegree { get; set; }
        public List<MoreQuestionDto> MoreQuestion { get; set; }

    }
    public class MoreQuestionDto
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public string ExamName { get; set; }
        public string Degree { get; set; }
    }
}