using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.SubscriptionsTypes
{
    public class SubscriptionsTypesServices
    {
        public List<SubscriptionsType> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.SubscriptionsTypes.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).ToList();
                return model;
            }
        }

        public ResultDto<SubscriptionsType> Create(SubscriptionsType model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<SubscriptionsType>();
                var Oldmodel = dbContext.SubscriptionsTypes.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "نوع الاشتراك موجود بالفعل";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.SubscriptionsTypes.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<SubscriptionsType> Edit(SubscriptionsType model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<SubscriptionsType>();
                var Oldmodel = dbContext.SubscriptionsTypes.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "نوع الاشتراك غير موجود ";
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
        public ResultDto<SubscriptionsType> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<SubscriptionsType>();
                var Oldmodel = dbContext.SubscriptionsTypes.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "نوع الاشتراك غير موجود ";
                    return result;
                }
                if (Oldmodel.Subscriptions.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = "نوع الاشتراك لا يمكن حذفه ";
                    return result;
                }
                var Oldmodel2 = dbContext.SubscriptionsTypes.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Take(3).ToList();
                if (Oldmodel2 != null)
                {
                    foreach (var item in Oldmodel2)
                    {
                        if (item.Id == Id)
                        {
                            result.IsSuccess = false;
                            result.Message = "نوع الاشتراك لا يمكن حذفه ";
                            return result;
                        }
                    }

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