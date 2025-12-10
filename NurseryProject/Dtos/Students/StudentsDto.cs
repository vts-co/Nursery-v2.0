using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Students
{
    public class StudentsDto
    {
        public Guid Id { get; set; }
        public Guid? StudyTypeId { get; set; }
        public Guid? StudyYearId { get; set; }
        public Guid? LevelId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid CityId { get; set; }
        public string CityName { get; set; }
        public Guid DestrictId { get; set; }
        public string DestrictName { get; set; }
        public string NumberId { get; set; }
        public Guid RegistrationTypeId { get; set; }
        public string RegistrationTypeName { get; set; }
        public string Image { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string OName { get; set; }

        
        public string BirthDate { get; set; }
        public string MotherName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string PhoneMo { get; set; }
        public string PhoneAth { get; set; }
        public string PhoneOwner { get; set; }
        public string FatherJob { get; set; }
        public string MotherJob { get; set; }
        public string FatherNumberId { get; set; }
        public string MotherNumberId { get; set; }
        public string JoiningDate { get; set; }
        public int GenderId { get; set; }
        public string GenderName { get; set; }
        public List<StudentFilesDto> StudentFiles { get; set; }

        public string Notes { get; set; }
    }

    public class StudentFilesDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
      
        public string Name { get; set; }
     
        public string Notes { get; set; }
    }

}