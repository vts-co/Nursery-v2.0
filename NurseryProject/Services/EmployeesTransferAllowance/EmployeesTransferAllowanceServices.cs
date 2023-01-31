using NurseryProject.Dtos.EmployeesTransferAllowance;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeesTransferAllowance
{
    public class EmployeesTransferAllowanceServices
    {
        public List<EmployeesTransferAllowanceDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesTransferAllowances.Where(x => x.IsDeleted == false&&x.Employee.IsDeleted==false).OrderBy(x => x.CreatedOn).Select(x => new EmployeesTransferAllowanceDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId.Value,
                    EmployeeName = x.Employee.Name,
                    Date = x.TransferDate.Value,
                    Value = x.TransferValue,
                }).ToList();
                return model;
            }
        }
        public Models.EmployeesTransferAllowance Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeesTransferAllowances.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Models.EmployeesTransferAllowance> Create(Models.EmployeesTransferAllowance model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesTransferAllowance>();

                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.EmployeesTransferAllowances.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesTransferAllowance> Edit(Models.EmployeesTransferAllowance model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesTransferAllowance>();
                var Oldmodel = dbContext.EmployeesTransferAllowances.Find(model.Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.TransferDate = model.TransferDate;
                Oldmodel.TransferValue = model.TransferValue;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesTransferAllowance> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesTransferAllowance>();
                var Oldmodel = dbContext.EmployeesTransferAllowances.Find(Id);

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