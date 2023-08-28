using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeesRegistrationTypes
{
    public class RegistrationTypesServices
    {
        public List<EmployeeRegistrationType> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeeRegistrationTypes.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).ToList();
                return model;
            }
        }

        public ResultDto<EmployeeRegistrationType> Create(EmployeeRegistrationType model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeRegistrationType>();
                var Oldmodel = dbContext.EmployeeRegistrationTypes.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "هذه الحالة موجودة بالفعل";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.EmployeeRegistrationTypes.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeeRegistrationType> Edit(EmployeeRegistrationType model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeRegistrationType>();
                var Oldmodel = dbContext.EmployeeRegistrationTypes.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الحالة موجودة بالفعل";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.Name = model.Name;
                Oldmodel.Notes = model.Notes;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeeRegistrationType> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeRegistrationType>();
                if(Id==Guid.Parse("E31AC343-47DA-4DFE-8970-E1719DEEC869") || Id==Guid.Parse("5cf58b9e-d1eb-41d8-9b11-4ccc120648a3")|| Id==Guid.Parse("B8650DC5-A83D-4A48-B99F-B75196A1DF8C"))
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الحالة لا يمكن حذفها ";
                    return result;
                }
                var Oldmodel = dbContext.EmployeeRegistrationTypes.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الحالة غير موجودة ";
                    return result;
                }
                if (Oldmodel.Employees.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الحالة بها موظفين لا يمكن حذفها ";
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