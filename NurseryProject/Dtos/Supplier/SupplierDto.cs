using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Supplier
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string Phone { get; set; }
    }
}