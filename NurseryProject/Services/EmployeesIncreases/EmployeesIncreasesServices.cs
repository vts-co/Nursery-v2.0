using NurseryProject.Dtos.EmployeesIncreases;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeesIncreases
{
    public class EmployeesIncreasesServices
    {
        public List<EmployeesIncreasesDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesIncreases.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new EmployeesIncreasesDto
                {
                    Id = x.Id,
                    IncreaseTypeId = x.IncreaseTypeId.Value,
                    IncreaseTypeName = x.IncreasesType.Name,
                    EmployeeId = x.EmployeeId.Value,
                    EmployeeName = x.Employee.Name,
                    Date = x.IncreaseDate.Value,
                    Value = x.IncreaseValue.Value.ToString(),
                    Reason = x.IncreaseReason
                }).ToList();
                return model;
            }
        }
        public Models.EmployeesIncreas Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesIncreases.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Models.EmployeesIncreas> Create(Models.EmployeesIncreas model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesIncreas>();

                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.EmployeesIncreases.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesIncreas> Edit(Models.EmployeesIncreas model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesIncreas>();
                var Oldmodel = dbContext.EmployeesIncreases.Find(model.Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.IncreaseDate = model.IncreaseDate;
                Oldmodel.IncreaseValue = model.IncreaseValue;
                Oldmodel.IncreaseReason = model.IncreaseReason;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesIncreas> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesIncreas>();
                var Oldmodel = dbContext.EmployeesIncreases.Find(Id);

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