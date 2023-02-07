using NurseryProject.Dtos.EmployeesWorkShifts;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeesWorkShifts
{
    public class EmployeesWorkShiftsServices
    {
        public List<EmployeesWorkShiftsDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesWorkShifts.Where(x => x.IsDeleted == false && x.WorkShift.IsDeleted == false && x.Employee.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new EmployeesWorkShiftsDto
                {
                    Id = x.Id,

                    StudyYearId = x.StudyYearId.Value,
                    StudyYearName = x.StudyYear.Name,
                    DepartmentId = x.Employee.Jop.DepartmentId.Value,
                    DepartmentName = x.Employee.Jop.Department.Name,
                    EmployeeId = x.Employee.Id,
                    EmployeeName = x.Employee.Name,
                    WorkShiftId = x.WorkShift.Id,
                    WorkShiftName = x.WorkShift.Name,
                    Notes = x.Notes
                }).ToList();
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
        public ResultDto<EmployeesWorkShift> Create(EmployeesWorkShift model, List<Guid> emp, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeesWorkShift>();
                foreach (var item in emp)
                {

                    var Oldmodel = dbContext.EmployeesWorkShifts.Where(x => x.WorkShiftId == model.WorkShiftId && x.EmployeeId == item && x.IsDeleted == false).FirstOrDefault();
                    if (Oldmodel == null)
                    {
                        EmployeesWorkShift employeesWorkShift = new EmployeesWorkShift();
                        employeesWorkShift.Id = Guid.NewGuid();
                        employeesWorkShift.CreatedOn = DateTime.UtcNow;
                        employeesWorkShift.CreatedBy = UserId;
                        employeesWorkShift.IsDeleted = false;
                        employeesWorkShift.EmployeeId = item;
                        employeesWorkShift.StudyYearId = model.StudyYearId;
                        employeesWorkShift.WorkShiftId = model.WorkShiftId;
                        employeesWorkShift.Notes = model.Notes;

                        dbContext.EmployeesWorkShifts.Add(employeesWorkShift);
                        dbContext.SaveChanges();
                    }
                }

                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeesWorkShift> Edit(EmployeesWorkShift model, List<Guid> emp, Guid UserId)
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
                var Oldmodel1 = dbContext.EmployeesWorkShifts.Where(x => x.WorkShiftId == model.WorkShiftId&&x.IsDeleted==false).ToList();
                foreach (var item in Oldmodel1)
                {
                    item.IsDeleted = true;
                    item.DeletedBy = UserId;
                    item.CreatedOn= DateTime.UtcNow;
                    dbContext.SaveChanges();
                }
                foreach (var item in emp)
                {
                    EmployeesWorkShift employeesWorkShift = new EmployeesWorkShift();
                    employeesWorkShift.Id = Guid.NewGuid();
                    employeesWorkShift.CreatedOn = DateTime.UtcNow;
                    employeesWorkShift.CreatedBy = UserId;
                    employeesWorkShift.IsDeleted = false;
                    employeesWorkShift.EmployeeId = item;
                    employeesWorkShift.StudyYearId = model.StudyYearId;
                    employeesWorkShift.WorkShiftId = model.WorkShiftId;
                    employeesWorkShift.Notes = model.Notes;

                    dbContext.EmployeesWorkShifts.Add(employeesWorkShift);
                    dbContext.SaveChanges();
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