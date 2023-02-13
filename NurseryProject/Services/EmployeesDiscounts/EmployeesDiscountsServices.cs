using NurseryProject.Dtos.EmployeesDiscounts;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeesDiscounts
{
    public class EmployeesDiscountsServices
    {
        public List<EmployeesDiscountsDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesDiscounts.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new EmployeesDiscountsDto
                {
                    Id = x.Id,
                    DiscountTypeId = x.DiscountTypeId.Value,
                    DiscountTypeName = x.DiscountsType.Name,
                    EmployeeId = x.EmployeeId.Value,
                    EmployeeName = x.Employee.Name,
                    Date = x.DiscountDate.Value,
                    Value = x.DiscountValue.Value.ToString(),
                    Reason = x.DiscountReason
                }).ToList();
                return model;
            }
        }
        public Models.EmployeesDiscount Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesDiscounts.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Models.EmployeesDiscount> Create(Models.EmployeesDiscount model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesDiscount>();

                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.EmployeesDiscounts.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesDiscount> Edit(Models.EmployeesDiscount model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesDiscount>();
                var Oldmodel = dbContext.EmployeesDiscounts.Find(model.Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.DiscountDate = model.DiscountDate;
                Oldmodel.DiscountValue = model.DiscountValue;
                Oldmodel.DiscountReason = model.DiscountReason;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesDiscount> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesDiscount>();
                var Oldmodel = dbContext.EmployeesDiscounts.Find(Id);

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