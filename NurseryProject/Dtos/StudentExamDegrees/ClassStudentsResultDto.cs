using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.StudentExamDegrees
{
    public class ClassStudentsResultDto
    {
        public string StudentName { get; set; }
        public double? Percentage { get; set; }
        public double? Total { get; set; }
        public List<ClassStudentsSubjectsResultDto> Details { get; set; }

    }
    public class ClassStudentsSubjectsResultDto
    {
        public string Name { get; set; }
        public double? Degree { get; set; }
        public double? ExamDegree { get; set; }
        public string StudentDegree { get; set; }

    }
}