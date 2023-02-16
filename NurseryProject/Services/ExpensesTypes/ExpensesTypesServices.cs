using NurseryProject.Dtos.ExpensesTypes;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.ExpensesTypes
{
    public class ExpensesTypesServices
    {
        public List<ExpensesTypesDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.ExpensesTypes.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new ExpensesTypesDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.ParentId == null ? Guid.Empty : x.ParentId.Value,
                    ParentName = x.ParentId == null ? "" : x.ExpensesType1.Name,
                    Notes=x.Notes
                }).ToList();
                return model;
            }
        }
        public ExpensesType Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.ExpensesTypes.Where(x => x.IsDeleted == false&&x.Id==Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
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
                Oldmodel.ParentId = model.ParentId;
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
                if (Id == Guid.Parse("20A6A59E-088E-4E6A-AA27-8A15F051B1DE")|| Id == Guid.Parse("A41366FB-D326-4723-A630-F81E38D5ED23"))
                {
                    result.IsSuccess = false;
                    result.Message = "نوع المصروف لا يمكن حذفها ";
                    return result;
                }
                if (Oldmodel.Expenses.Any(y => y.IsDeleted == false)|| Oldmodel.ExpensesTypes1.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = "نوع المصروف لا يمكن حذفها ";
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