using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.ExpensesTypes
{
    public class ExpensesTypesServices
    {
        public List<ExpensesType> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.ExpensesTypes.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).ToList();
                return model;
            }
        }

        public ResultDto<ExpensesType> Create(ExpensesType model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<ExpensesType>();
                var Oldmodel = dbContext.ExpensesTypes.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "نوع المصروف موجود بالفعل";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.ExpensesTypes.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<ExpensesType> Edit(ExpensesType model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<ExpensesType>();
                var Oldmodel = dbContext.ExpensesTypes.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "نوع المصروف غير موجود ";
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
        public ResultDto<ExpensesType> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<ExpensesType>();
                var Oldmodel = dbContext.ExpensesTypes.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "نوع المصروف غير موجود ";
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