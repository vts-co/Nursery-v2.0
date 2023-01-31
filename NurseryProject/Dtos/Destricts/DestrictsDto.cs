using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Destricts
{
    public class DestrictsDto
    {
        public Guid Id { get; set; }
        public Guid CityId { get; set; }
        public string CityName { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }

    }
}