using NurseryProject.Dtos.ShiftsTimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.WorkShifts
{
    public class WorkShiftsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public List<ShiftsTimesDto> ShiftTimes { get; set; }

    }
}