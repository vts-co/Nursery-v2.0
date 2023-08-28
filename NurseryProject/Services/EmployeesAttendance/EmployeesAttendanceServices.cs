using NurseryProject.Controllers;
using NurseryProject.Dtos.EmployeesAttendance;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeesAttendance
{
    public class EmployeesAttendanceServices
    {
        public List<EmployeesAttendanceDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesAttendances.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new EmployeesAttendanceDto
                {
                    Id = x.Id,
                    EmployeeWorkShiftId = x.EmployeeWorkShiftId.Value,
                    StudyYearId = x.EmployeesWorkShift.StudyYearId.Value,
                    StudyYearName = x.EmployeesWorkShift.StudyYear.Name,
                    DepartmentId = x.EmployeesWorkShift.Employee.Jop.DepartmentId.Value,
                    DepartmentName = x.EmployeesWorkShift.Employee.Jop.Department.Name,
                    EmployeeId = x.EmployeesWorkShift.Employee.Id,
                    EmployeeName = x.EmployeesWorkShift.Employee.Name,
                    Code = x.EmployeesWorkShift.Employee.Code,
                    WorkShiftId = x.EmployeesWorkShift.WorkShift.Id,
                    WorkShiftName = x.EmployeesWorkShift.WorkShift.Name,
                    IsAttend = x.IsAttend.Value,
                    Date = x.Date.Value.ToString(),
                    All = "",
                    Attend = ""
                }).ToList();
                var model1 = model.Count();

                var mod = new List<EmployeesAttendanceDto>();

                if (model.Count() > 0)
                {
                    var date = model[0].Date;
                    var class1 = model[0].WorkShiftId;
                    var class2 = model[0].StudyYearId;

                    for (int i = 1; i < model1; i++)
                    {
                        if (date == model[i].Date && class1 == model[i].WorkShiftId && class2 == model[i].StudyYearId)
                        {
                            model.Remove(model[i]);
                            i--;
                            model1 -= 1;
                        }
                        else
                        {
                            date = model[i].Date;
                            class1 = model[i].WorkShiftId;
                            class2 = model[i].StudyYearId;

                        }
                    }
                    foreach (var item in model)
                    {
                        var date1 = DateTime.Parse(item.Date).Date;
                        var model2 = dbContext.EmployeesWorkShifts.Where(x => x.IsDeleted == false && x.WorkShiftId == item.WorkShiftId && x.StudyYearId == item.StudyYearId).ToList();
                        var model3 = dbContext.EmployeesAttendances.Where(x => x.IsDeleted == false && x.Date == date1 && x.EmployeesWorkShift.WorkShiftId == item.WorkShiftId && x.EmployeesWorkShift.StudyYearId == item.StudyYearId && x.IsAttend == true).ToList();
                        item.All = model2.Count().ToString();
                        item.Attend = model3.Count().ToString();
                    };
                }
                return model;
            }
        }
        public EmployeesAttendanceDto Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesAttendances.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).Select(x => new EmployeesAttendanceDto
                {
                    Id = x.Id,
                    EmployeeWorkShiftId = x.EmployeeWorkShiftId.Value,
                    StudyYearId = x.EmployeesWorkShift.StudyYearId.Value,
                    StudyYearName = x.EmployeesWorkShift.StudyYear.Name,
                    DepartmentId = x.EmployeesWorkShift.Employee.Jop.DepartmentId.Value,
                    DepartmentName = x.EmployeesWorkShift.Employee.Jop.Department.Name,
                    EmployeeId = x.EmployeesWorkShift.Employee.Id,
                    EmployeeName = x.EmployeesWorkShift.Employee.Name,
                    WorkShiftId = x.EmployeesWorkShift.WorkShift.Id,
                    WorkShiftName = x.EmployeesWorkShift.WorkShift.Name,
                    IsAttend = x.IsAttend.Value,
                    Date = x.Date.Value.ToString(),
                    All = "",
                    Attend = ""
                }).FirstOrDefault();
                return model;
            }
        }
        public List<EmployeesAttendanceDto> GetByEmployeeWorkShift()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesAttendances.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new EmployeesAttendanceDto
                {
                    Id = x.Id,
                    EmployeeWorkShiftId = x.EmployeeWorkShiftId.Value,
                    StudyYearId = x.EmployeesWorkShift.StudyYearId.Value,
                    StudyYearName = x.EmployeesWorkShift.StudyYear.Name,
                    
                    DepartmentId = x.EmployeesWorkShift.Employee.Jop.DepartmentId.Value,
                    DepartmentName = x.EmployeesWorkShift.Employee.Jop.Department.Name,
                    EmployeeId = x.EmployeesWorkShift.Employee.Id,
                    EmployeeName = x.EmployeesWorkShift.Employee.Name,
                    EmployeePhone=x.EmployeesWorkShift.Employee.Phone,
                    WorkShiftId = x.EmployeesWorkShift.WorkShift.Id,
                    WorkShiftName = x.EmployeesWorkShift.WorkShift.Name,
                    IsAttend = x.IsAttend.Value,
                    Date = x.Date.Value.ToString(),
                    All = "",
                    Attend = ""
                }).ToList();
                return model;
            }
        }
        public ResultDto<Models.EmployeesAttendance> Create(Models.EmployeesAttendance model, List<Attend1> emp, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesAttendance>();
                
                foreach (var item in emp)
                {
                    var Oldmodel = dbContext.EmployeesAttendances.Where(x => x.IsDeleted == false && x.EmployeeWorkShiftId == item.Id && x.Date == model.Date).OrderBy(x => x.CreatedOn).FirstOrDefault();
                    if (Oldmodel != null)
                    {
                        result.IsSuccess = false;
                        result.Message = "تم اخذ الغياب لهذا اليوم";
                        return result;
                    }
                }
               
                foreach (var item in emp)
                {
                    Models.EmployeesAttendance employeesWorkShiftAttendance = new Models.EmployeesAttendance();
                    employeesWorkShiftAttendance.Id = Guid.NewGuid();
                    employeesWorkShiftAttendance.CreatedOn = DateTime.UtcNow;
                    employeesWorkShiftAttendance.CreatedBy = UserId;
                    employeesWorkShiftAttendance.IsDeleted = false;
                    employeesWorkShiftAttendance.EmployeeWorkShiftId = item.Id;
                    employeesWorkShiftAttendance.Date = model.Date;
                    if (item.Att == "on")
                    {
                        employeesWorkShiftAttendance.IsAttend = true;
                    }
                    else
                    {
                        employeesWorkShiftAttendance.IsAttend = false;
                    }
                    dbContext.EmployeesAttendances.Add(employeesWorkShiftAttendance);
                    dbContext.SaveChanges();
                }


                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesAttendance> Edit(Models.EmployeesAttendance model, List<Attend1> emp, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesAttendance>();

                var Oldmodel1 = Get(model.Id);

                var Oldmodel4 = dbContext.EmployeesAttendances.Where(x => x.EmployeesWorkShift.WorkShiftId == Oldmodel1.WorkShiftId && x.EmployeesWorkShift.StudyYearId == Oldmodel1.StudyYearId && x.Date.ToString() == Oldmodel1.Date && x.IsDeleted == false).ToList();
                if (Oldmodel4 != null)
                {
                    foreach (var item in Oldmodel4)
                    {
                        item.IsDeleted = true;
                        item.DeletedOn = DateTime.UtcNow;
                        item.DeletedBy = UserId;
                        dbContext.SaveChanges();
                    }
                }

                foreach (var item2 in emp)
                {
                    Models.EmployeesAttendance employeesWorkShiftAttendance = new Models.EmployeesAttendance();
                    employeesWorkShiftAttendance.Id = Guid.NewGuid();
                    employeesWorkShiftAttendance.CreatedOn = DateTime.UtcNow;
                    employeesWorkShiftAttendance.CreatedBy = UserId;
                    employeesWorkShiftAttendance.IsDeleted = false;
                    employeesWorkShiftAttendance.EmployeeWorkShiftId = item2.Id;
                    employeesWorkShiftAttendance.Date = model.Date;
                    if (item2.Att == "on")
                    {
                        employeesWorkShiftAttendance.IsAttend = true;
                    }
                    else
                    {
                        employeesWorkShiftAttendance.IsAttend = false;
                    }
                    dbContext.EmployeesAttendances.Add(employeesWorkShiftAttendance);
                    dbContext.SaveChanges();
                }

                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesAttendance> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesAttendance>();
                var Oldmodel = dbContext.EmployeesAttendances.Find(Id);
                var Oldmodel1 = dbContext.EmployeesAttendances.Where(x => x.EmployeesWorkShift.StudyYearId == Oldmodel.EmployeesWorkShift.StudyYearId && x.EmployeesWorkShift.WorkShiftId == Oldmodel.EmployeesWorkShift.WorkShiftId && x.Date == Oldmodel.Date && x.IsDeleted == false).ToList();
                foreach (var item in Oldmodel1)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    item.DeletedBy = UserId;
                    dbContext.SaveChanges();
                }

                result.IsSuccess = true;
                result.Message = "تم حذف البيانات بنجاح";
                return result;
            }
        }
    }
}