using NurseryProject.Dtos.ShiftsTimes;
using NurseryProject.Dtos.WorkShifts;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.WorkShifts
{
    public class WorkShiftsServices
    {
        public List<WorkShiftsDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.WorkShifts.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.EmployeesWorkShifts.Any(y=>y.IsDeleted == false && y.EmployeeId == EmployeeId) || x.EmployeesWorkShifts.Any(p => p.IsDeleted == false && p.Employee.BuildingSupervisors.Any(k => k.IsDeleted == false && k.EmployeeId == EmployeeId)))).OrderBy(x => x.CreatedOn).Select(x => new WorkShiftsDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Notes = x.Notes,
                    ShiftTimes = x.ShiftsTimes.Where(y => y.WorkShiftId == x.Id && y.IsDeleted == false).OrderBy(y => y.CreatedOn).Select(y => new ShiftsTimesDto
                    {
                        Id = y.Id,
                        DayName = y.DayName,
                        TimeFrom = y.TimeFrom,
                        TimeTo = y.TimeTo
                    }).ToList()
                }).ToList();
                foreach (var item in model)
                {
                    foreach (var item2 in item.ShiftTimes)
                    {
                        if(item2!=null)
                        {
                            item2.TimeFrom = DateTime.Parse(item2.TimeFrom).ToString("hh:mm tt");
                            item2.TimeTo = DateTime.Parse(item2.TimeTo).ToString("hh:mm tt");
                        }
                       
                    }
                }
                return model;
            }
        }

        public WorkShiftsDto Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.WorkShifts.Where(x => x.IsDeleted == false &&x.Id==Id).OrderBy(x => x.CreatedOn).Select(x => new WorkShiftsDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Notes = x.Notes,
                    ShiftTimes = x.ShiftsTimes.Where(y => y.WorkShiftId == x.Id && y.IsDeleted==false).OrderBy(y => y.CreatedOn).Select(y => new ShiftsTimesDto
                    {
                        Id = y.Id,
                        IsFound=true,
                        DayName = y.DayName,
                        TimeFrom = y.TimeFrom,
                        TimeTo = y.TimeTo
                    }).ToList()
                }).FirstOrDefault();
                List<string> days = new List<string> { "الجمعة", "السبت", "الاحد", "الاتنين", "الثلاثاء", "الاربعاء", "الخميس" };

                List<ShiftsTimesDto> shiftsTimes = new List<ShiftsTimesDto>();
                shiftsTimes = model.ShiftTimes;

                List<ShiftsTimesDto> shiftsTimes2 = new List<ShiftsTimesDto>();
                for (int i = 0; i < 7; i++)
                {
                    shiftsTimes2.Add(new ShiftsTimesDto
                    {
                        Id = Guid.Empty,
                        IsFound = false,
                        DayName = days[i],
                        TimeFrom = null,
                        TimeTo = null
                    });
                }

                model.ShiftTimes = new List<ShiftsTimesDto>();

                foreach (var item in shiftsTimes2)
                {
                    foreach (var item2 in shiftsTimes)
                    {
                        if (item.DayName == item2.DayName)
                        {
                            item.Id = item2.Id;
                            item.DayName = item2.DayName;
                            item.IsFound = item2.IsFound;
                            item.TimeFrom = item2.TimeFrom;
                            item.TimeTo = item2.TimeTo;
                        }
                    }
                }
                model.ShiftTimes = shiftsTimes2;
                return model;
            }
        }

        public ResultDto<WorkShiftsDto> Create(WorkShiftsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<WorkShiftsDto>();
                var Oldmodel = dbContext.WorkShifts.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الفترة موجودة بالفعل";
                    return result;
                }
                WorkShift workShifts = new WorkShift()
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Notes = model.Notes,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = UserId,
                    IsDeleted = false
                };

                dbContext.WorkShifts.Add(workShifts);

                foreach (var item in model.ShiftTimes)
                {
                    if (item.IsFound)
                    {
                        ShiftsTime shiftsTime = new ShiftsTime()
                        {
                            Id = Guid.NewGuid(),
                            WorkShiftId = workShifts.Id,
                            DayName = item.DayName,
                            TimeFrom = item.TimeFrom,
                            TimeTo = item.TimeTo,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = UserId,
                            IsDeleted = false
                        };
                        dbContext.ShiftsTimes.Add(shiftsTime);
                    }

                }

                dbContext.SaveChanges();

                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<WorkShiftsDto> Edit(WorkShiftsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<WorkShiftsDto>();
                var Oldmodel = dbContext.WorkShifts.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الفترة غير موجودة ";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.Name = model.Name;
                Oldmodel.Notes = model.Notes;
                dbContext.SaveChanges();

                var Oldmodel2 = dbContext.ShiftsTimes.Where(x => x.WorkShiftId == model.Id && x.IsDeleted == false).OrderBy(x => x.CreatedOn).ToList();
                if (Oldmodel2 == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الفترة غير موجودة ";
                    return result;
                }
                for (int i = 0; i < Oldmodel2.Count(); i++)
                {
                    Oldmodel2[i].IsDeleted = true;
                    Oldmodel2[i].DeletedOn = DateTime.UtcNow;
                    Oldmodel2[i].DeletedBy = UserId;
                    dbContext.SaveChanges();

                }
                foreach (var item in model.ShiftTimes)
                {
                    if (item.IsFound)
                    {
                        ShiftsTime shiftsTime = new ShiftsTime()
                        {
                            Id = Guid.NewGuid(),
                            WorkShiftId = model.Id,
                            DayName = item.DayName,
                            TimeFrom = item.TimeFrom,
                            TimeTo = item.TimeTo,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = UserId,
                            IsDeleted = false
                        };
                        dbContext.ShiftsTimes.Add(shiftsTime);
                        dbContext.SaveChanges();

                    }
                }
               
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<WorkShiftsDto> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<WorkShiftsDto>();

                var Oldmodel = dbContext.WorkShifts.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الفترة غير موجودة ";
                    return result;
                }
                if (Oldmodel.EmployeesWorkShifts.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الفترة لا يمكن حذفها ";
                    return result;
                }
                var Oldmodel2 = dbContext.ShiftsTimes.Where(x => x.WorkShiftId == Id && x.IsDeleted == false).OrderBy(x => x.CreatedOn).ToList();
                if (Oldmodel2 != null)
                {
                    foreach (var item in Oldmodel2)
                    {
                        item.IsDeleted = true;
                        item.DeletedOn = DateTime.UtcNow;
                        item.DeletedBy = UserId;
                    }
                }

                Oldmodel.IsDeleted = true;
                Oldmodel.DeletedOn = DateTime.UtcNow;
                Oldmodel.DeletedBy = UserId;
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حذف البيانات بنجاح";
                return result;
            }
        }
    }
}