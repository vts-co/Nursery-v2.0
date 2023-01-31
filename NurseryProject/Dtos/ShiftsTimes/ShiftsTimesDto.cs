using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.ShiftsTimes
{
    public class ShiftsTimesDto
    {
        public Guid Id { get; set; }
        public string DayName { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public bool IsFound { get; set; }

    }
}