using NurseryProject.Dtos.EmployeesDelay;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeesDelay
{
    public class EmployeesDelayServices
    {
        public List<EmployeesDelayDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesDelays.Where(x => x.IsDeleted == false&&x.Employee.IsDeleted==false).OrderBy(x => x.CreatedOn).Select(x => new EmployeesDelayDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId.Value,
                    EmployeeName = x.Employee.Name,
                    Date = x.DelayDate.Value,
                    Value = x.DelayValue,
                    Reason = x.DelayReason
                }).ToList();
                return model;
            }
        }
        public Models.EmployeesDelay Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesDelays.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Models.EmployeesDelay> Create(Models.EmployeesDelay model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesDelay>();

                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.EmployeesDelays.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesDelay> Edit(Models.EmployeesDelay model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesDelay>();
                var Oldmodel = dbContext.EmployeesDelays.Find(model.Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.DelayDate = model.DelayDate;
                Oldmodel.DelayValue = model.DelayValue;
                Oldmodel.DelayReason = model.DelayReason;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesDelay> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesDelay>();
                var Oldmodel = dbContext.EmployeesDelays.Find(Id);

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