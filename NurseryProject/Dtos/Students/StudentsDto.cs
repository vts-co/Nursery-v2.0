using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Students
{
    public class StudentsDto
    {
        public Guid Id { get; set; }
        public Guid CityId { get; set; }
        public string CityName { get; set; }
        public Guid DestrictId { get; set; }
        public string DestrictName { get; set; }
        public Guid RegistrationTypeId { get; set; }
        public string RegistrationTypeName { get; set; }
        public string Image { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string BirthDate { get; set; }
        public string MotherName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string JoiningDate { get; set; }
        public int GenderId { get; set; }
        public string GenderName { get; set; }

        public string Notes { get; set; }
    }
}