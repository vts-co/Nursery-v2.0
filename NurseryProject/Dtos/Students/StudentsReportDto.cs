using NurseryProject.Dtos.EmployeeClasses;
using NurseryProject.Dtos.PaymentDetails;
using NurseryProject.Dtos.StudentsAttendance;
using NurseryProject.Dtos.StudentsClass;
using NurseryProject.Dtos.StudentsClassesTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Students
{
    public class StudentsReportDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string CountOfLatecomers { get; set; }
        public string CountOfNormal { get; set; }
        public string CountOfPart { get; set; }
        public string CountOfPaidNoTime { get; set; }

        public string AttendanceNum { get; set; }
        public string NoAttendanceNum { get; set; }
        public Guid ClassId { get; set; }
        public Guid LevelId { get; set; }
        public string ClassName { get; set; }
        public Guid StudentClassId { get; set; }

        public string CountOfTransferClasses { get; set; }
        public List<StudentsClassesTransferDto> TransferClasses { get; set; }
        public string ExamsRate { get; set; }
        public List<EmployeeClassesDto> Employees { get; set; }
        public List<StudentsAttendanceDaysDto> AttendanceDetails { get; set; }
        public List<SubscriptionMethodDto> PaidDetails { get; set; }

    }
    
}