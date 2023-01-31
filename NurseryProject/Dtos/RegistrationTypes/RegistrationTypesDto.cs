using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.RegistrationTypes
{
    public class RegistrationTypesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}