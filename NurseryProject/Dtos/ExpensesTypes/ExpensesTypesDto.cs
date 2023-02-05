using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.ExpensesTypes
{
    public class ExpensesTypesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ParentId { get; set; }
        public string ParentName { get; set; }
        public string Notes { get; set; }
    }
}