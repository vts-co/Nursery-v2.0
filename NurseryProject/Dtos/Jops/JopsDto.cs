using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Jops
{
    public class JopsDto
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}