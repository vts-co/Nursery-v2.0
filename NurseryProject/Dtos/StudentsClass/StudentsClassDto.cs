using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.StudentsClass
{
    public class StudentsClassDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
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
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string StudentPhone { get; set; }

        public Guid? SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public string Regular { get; set; }
        public string Collector { get; set; }

        public List<SubscriptionMethodDto> SubscriptionMethod { get; set; }
        public bool IsCurrent { get; set; }

        public string Amount { get; set; }
        public string Number { get; set; }
        public bool IsAnother { get; set; }
        public string JoiningDate { get; set; }
        public string Notes { get; set; }

        public List<StudentsClassDto> StudentsClassPrevious { get; set; }

        public string Paid { get; set; }
        public string Remain { get; set; }

        
        public string Date { get; set; }
    }
    public class SubscriptionMethodDto
    {
        public Guid Id { get; set; }
        public Guid StudentClassId { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
        public string PaidAmount { get; set; }
        public string Collector { get; set; }
        
        public bool IsPaid { get; set; }
        public string Paided { get; set; }

        public string PaidDate { get; set; }
        public string PaperNumber1 { get; set; }
        public string PaperNumber2 { get; set; }
    }
public  class DailySubscriptionMethod
    {
        public string Name { get; set; }
        public double Amount { get; set; }
    }

}