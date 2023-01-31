using NurseryProject.Dtos.Subscriptions;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.Subscriptions
{
    public class SubscriptionsServices
    {
        public List<SubscriptionsDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Subscriptions.Where(x => x.IsDeleted == false&&x.SubscriptionsType.IsDeleted==false&&x.Level.IsDeleted==false).OrderBy(x => x.CreatedOn).Select(x => new SubscriptionsDto
                {
                    Id = x.Id,
                    StudyTypeId = x.Level.StudyTypeId.Value,
                    StudyTypeName = x.Level.StudyType.Name,
                    LevelId = x.LevelId.Value,
                    LevelName = x.Level.Name,
                    SubscriptionTypeId = x.SubscriptionsType.Id,
                    SubscriptionTypeName = x.SubscriptionsType.Name,
                    Amount = x.Amount,
                    InstallmentsNumber = x.InstallmentsNumber,
                }).ToList();
                //foreach (var item in model)
                //{
                //    if (!item.IsAnother)
                //    {
                //        item.InstallmentAmount = (int.Parse(item.Amount) / int.Parse(item.InstallmentsNumber)).ToString();
                //    }

                //}
                return model;
            }
        }
        public Subscription Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Subscriptions.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Subscription> Create(Subscription model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Subscription>();
                var Oldmodel = dbContext.Subscriptions.Where(x => x.SubscriptionTypeId == model.SubscriptionTypeId &&x.LevelId==model.LevelId&& x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "هذا الاشتراك موجود بالفعل";
                    return result;
                }
                
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.Subscriptions.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Subscription> Edit(Subscription model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Subscription>();
                var Oldmodel = dbContext.Subscriptions.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الاشتراك غير موجود ";
                    return result;
                }
                var Oldmodel2 = dbContext.Subscriptions.Where(x => x.SubscriptionTypeId == model.SubscriptionTypeId && x.LevelId == model.LevelId && x.IsDeleted == false &&x.Id!= model.Id).ToList();
                if (Oldmodel2.Count() >0)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الاشتراك موجود بالفعل";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.LevelId = model.LevelId;
                Oldmodel.SubscriptionTypeId = model.SubscriptionTypeId;

                Oldmodel.Amount = model.Amount;
                Oldmodel.InstallmentsNumber = model.InstallmentsNumber;
                
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Subscription> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Subscription>();
                var Oldmodel = dbContext.Subscriptions.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الاشتراك غير موجود ";
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