using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.SubscriptionsMethods
{
    public class SubscriptionsMethodsServices
    {
        public List<SubscriptionMethod> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.SubscriptionMethods.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).ToList();
                return model;
            }
        }

        public bool Update(Guid Id, string Amount, string Date, string Id2, float Sub, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.SubscriptionMethods.Find(Id);
                var model2 = dbContext.SubscriptionMethods.Where(x => x.IsDeleted == false && x.OrderDisplay == model.OrderDisplay - 1 && x.IsPaid == false && x.StudentClassId == model.StudentClassId).FirstOrDefault();

                if (model2 == null)
                {
                    if (Id2 != null && Sub > 0)
                    {
                        var id = Guid.Parse(Id2);
                        var model1 = dbContext.SubscriptionMethods.Find(id);

                        var mod = float.Parse(model1.Amount);
                        mod += Sub;
                        model1.Amount = mod.ToString();
                        dbContext.SaveChanges();

                    }

                    model.IsPaid = true;
                    if (Date != null)
                    {
                        model.PaidDate = DateTime.Parse(Date);
                    }
                    model.PaidAmount = Amount;


                    model.ModifiedOn = DateTime.UtcNow;
                    model.ModifiedBy = UserId;
                    dbContext.SaveChanges();
                    return true;

                }
                else
                {
                    return false;
                }
            }
        }
    }
}