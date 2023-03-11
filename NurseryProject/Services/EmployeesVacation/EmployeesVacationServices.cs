using NurseryProject.Dtos.EmployeesVacation;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeesVacation
{
    public class EmployeesVacationServices
    {
        public List<EmployeesVacationDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesVacations.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.EmployeeId == EmployeeId || x.Employee.BuildingSupervisors.Any(y => y.IsDeleted == false && y.EmployeeId == EmployeeId))).OrderBy(x => x.CreatedOn).Select(x => new EmployeesVacationDto
                {
                    Id = x.Id,
                    VacationTypeId=x.VacationTypeId.Value,
                    VacationTypeName=x.VacationsType.Name,
                    EmployeeId = x.EmployeeId.Value,
                    EmployeeName = x.Employee.Name,
                    DateFrom = x.DateFrom.Value,
                    DateTo = x.DateTo.Value,
                    Reason=x.Reason
                }).ToList();
                return model;
            }
        }
        public Models.EmployeesVacation Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesVacations.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Models.EmployeesVacation> Create(Models.EmployeesVacation model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesVacation>();
               
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.EmployeesVacations.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesVacation> Edit(Models.EmployeesVacation model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesVacation>();
                var Oldmodel = dbContext.EmployeesVacations.Find(model.Id);
                
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.VacationTypeId = model.VacationTypeId;

                Oldmodel.DateFrom = model.DateFrom;
                Oldmodel.DateTo = model.DateTo;
                Oldmodel.Reason = model.Reason;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesVacation> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesVacation>();
                var Oldmodel = dbContext.EmployeesVacations.Find(Id);
               
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