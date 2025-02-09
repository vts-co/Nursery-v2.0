using NurseryProject.Controllers;
using NurseryProject.Dtos.EmployeesWorkShifts;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeesWorkShifts
{
    public class EmployeesWorkShiftsServices
    {
        public List<EmployeesWorkShiftsDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
             {
                var model = dbContext.EmployeesWorkShifts.Where(x => x.IsDeleted == false && !x.Employee.EmployeesVacations.Any(y => !y.IsDeleted && y.DateFrom <= DateTime.Now && y.DateTo <= DateTime.Now) && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.EmployeeId == EmployeeId || dbContext.BuildingSupervisors.Any(y => y.IsDeleted == false && y.EmployeeId == EmployeeId))).OrderBy(x => x.CreatedOn).Select(x => new EmployeesWorkShiftsDto
                {
                    Id = x.Id,
                    StudyYearId = x.StudyYearId.Value,
                    StudyYearName = x.StudyYear.Name,
                    DepartmentId = x.Employee.Jop.DepartmentId.Value,
                    DepartmentName = x.Employee.Jop.Department.Name,
                    EmployeeId = x.Employee.Id,
                    EmployeeName = x.Employee.Name,
                    Code = x.Employee.Code,
                    WorkShiftId = x.WorkShift.Id,
                    WorkShiftName = x.WorkShift.Name,
                    Notes = x.Notes,

                }).ToList();
                foreach (var item in model)
                {
                    var emp = dbContext.EmployeeClasses.Where(y => !y.IsDeleted && y.EmployeeId == item.EmployeeId).FirstOrDefault();
                    if(emp!=null)
                    {
                        var class1=dbContext.Classes.Where(y => !y.IsDeleted && y.Id == emp.ClassId).FirstOrDefault();
                        item.StudyPlaceId = (Guid)class1.StudyPlaceId;
                        item.StudyPlaceName = class1.StudyPlace.Name;
                    }
                    var emp2 = dbContext.BuildingSupervisors.Where(y => !y.IsDeleted && y.EmployeeId == item.EmployeeId).FirstOrDefault();
                    if (emp2 != null)
                    {
                        item.StudyPlaceId = (Guid)emp2.StudyPlaceId;
                        item.StudyPlaceName = emp2.StudyPlace.Name;
                    }
                    var emp3 = dbContext.ClassesLeaders.Where(y => !y.IsDeleted && y.EmployeeId == item.EmployeeId).FirstOrDefault();
                    if (emp3 != null)
                    {
                        var class1 = dbContext.Classes.Where(y => !y.IsDeleted && y.Id == emp3.ClassId).FirstOrDefault();
                        item.StudyPlaceId = (Guid)class1.StudyPlaceId;
                        item.StudyPlaceName = class1.StudyPlace.Name;
                    }
                }

                return model;
            }
        }
        public EmployeesWorkShift Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesWorkShifts.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<EmployeesWorkShift> Create(EmployeesWorkShift model, List<Attend2> emp, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeesWorkShift>();
                foreach (var item in emp)
                {
                    if (item.Att == "on")
                    {
                        var Oldmodel = dbContext.EmployeesWorkShifts.Where(x => x.WorkShiftId == model.WorkShiftId && x.StudyYearId == model.StudyYearId && x.EmployeeId == item.Id && x.IsDeleted == false).FirstOrDefault();
                        if (Oldmodel == null)
                        {
                            EmployeesWorkShift employeesWorkShift = new EmployeesWorkShift();
                            employeesWorkShift.Id = Guid.NewGuid();
                            employeesWorkShift.CreatedOn = DateTime.UtcNow;
                            employeesWorkShift.CreatedBy = UserId;
                            employeesWorkShift.IsDeleted = false;
                            employeesWorkShift.EmployeeId = item.Id;
                            employeesWorkShift.StudyYearId = model.StudyYearId;
                            employeesWorkShift.WorkShiftId = model.WorkShiftId;
                            employeesWorkShift.Notes = model.Notes;

                            dbContext.EmployeesWorkShifts.Add(employeesWorkShift);
                            dbContext.SaveChanges();
                        }
                    }
                }

                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeesWorkShift> Edit(EmployeesWorkShift model, List<Attend2> emp, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeesWorkShift>();
                //var Oldmodel = dbContext.EmployeesWorkShifts.Find(model.Id);
                //if (Oldmodel != null)
                //{
                //    var Oldmodel2 = dbContext.EmployeesWorkShifts.Where(x => x.WorkShiftId == model.WorkShiftId && x.Id != model.Id && x.EmployeeId == model.EmployeeId && x.IsDeleted == false).FirstOrDefault();
                //    if (Oldmodel2 != null)
                //    {
                //        result.Result = model;
                //        result.IsSuccess = false;
                //        result.Message = "هذا الموظف موجود بالفعل في هذه الفترة";
                //        return result;
                //    }
                //}
                var Oldmodel1 = dbContext.EmployeesWorkShifts.Where(x => x.WorkShiftId == model.WorkShiftId && x.StudyYearId == model.StudyYearId && x.IsDeleted == false).ToList();
                foreach (var item in Oldmodel1)
                {
                    item.IsDeleted = true;
                    item.DeletedBy = UserId;
                    item.CreatedOn = DateTime.UtcNow;
                    dbContext.SaveChanges();
                }
                foreach (var item in emp)
                {
                    if (item.Att == "on")
                    {
                        EmployeesWorkShift employeesWorkShift = new EmployeesWorkShift();
                        employeesWorkShift.Id = Guid.NewGuid();
                        employeesWorkShift.CreatedOn = DateTime.UtcNow;
                        employeesWorkShift.CreatedBy = UserId;
                        employeesWorkShift.IsDeleted = false;
                        employeesWorkShift.EmployeeId = item.Id;
                        employeesWorkShift.StudyYearId = model.StudyYearId;
                        employeesWorkShift.WorkShiftId = model.WorkShiftId;
                        employeesWorkShift.Notes = model.Notes;

                        dbContext.EmployeesWorkShifts.Add(employeesWorkShift);
                        dbContext.SaveChanges();
                    }

                }

                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeesWorkShift> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeesWorkShift>();
                var Oldmodel = dbContext.EmployeesWorkShifts.Find(Id);
                if (Oldmodel.EmployeesAttendances.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الفترة لا يمكن حذفها  ";
                    return result;
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