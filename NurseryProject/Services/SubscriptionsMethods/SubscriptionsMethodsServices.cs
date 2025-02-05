using NurseryProject.Models;
using NurseryProject.Services.Revenues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.SubscriptionsMethods
{
    public class SubscriptionsMethodsServices
    {
        RevenuesServices revenuesServices = new RevenuesServices();

        public List<SubscriptionMethod> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.SubscriptionMethods.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).ToList();
                return model;
            }
        }

        public bool Update(Guid Id, string Amount, string Date, string Id2, float Sub, string PaperNumber1, string PaperNumber2, Guid UserId)
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
                        model1.ModifiedBy = UserId;
                        dbContext.SaveChanges();

                    }

                    model.IsPaid = true;
                    model.PaperNumber1 = PaperNumber1;
                    model.PaperNumber2 = PaperNumber2;
                    model.IsPaid = true;
                    if (Date != null)
                    {
                        model.PaidDate = DateTime.Parse(Date);
                    }
                    model.PaidAmount = Amount;

                    model.ModifiedOn = DateTime.UtcNow;
                    model.ModifiedBy = UserId;
                    dbContext.SaveChanges();

                    var Revenue = new Revenue
                    {
                        Id = Guid.NewGuid(),
                        StudyYearId = model.StudentsClass.StudyYearId,
                        StudyPlaceId = model.StudentsClass.Class.StudyPlaceId,
                        RevenueTypeId = Guid.Parse("0F54C94D-B010-4360-8EB1-B93E05615065"),
                        RevenueValue =double.Parse( Amount),
                        RevenueDate = model.PaidDate,
                        Notes = "تحصيل اشتراك الطالب " + model.StudentsClass.Student.Name,
                        EmployeeId = null,
                        SubscriptionMethodId = Id
                    };
                    revenuesServices.Create(Revenue, UserId);

                    return true;

                }
                else
                {
                    return false;
                }
            }
        }


        public bool Update2(Guid Id, string Amount, string Date, string Id2, float Sub, string PaperNumber1, string PaperNumber2,string NewpaymentDate, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.SubscriptionMethods.Find(Id);
                var model2 = dbContext.SubscriptionMethods.Where(x => x.IsDeleted == false && x.OrderDisplay == model.OrderDisplay - 1 && x.IsPaid == false && x.StudentClassId == model.StudentClassId).FirstOrDefault();

                SubscriptionMethod subscriptionMethod = new SubscriptionMethod();
                subscriptionMethod.Id = Guid.NewGuid();
                subscriptionMethod.Amount =Sub.ToString();
                subscriptionMethod.Date =DateTime.Parse( NewpaymentDate);
                subscriptionMethod.PaidAmount ="0";
                subscriptionMethod.IsPaid =false;
                subscriptionMethod.OrderDisplay = model.OrderDisplay+1;
                subscriptionMethod.StudentClassId = model.StudentClassId;
                dbContext.SubscriptionMethods.Add(subscriptionMethod);
                dbContext.SaveChanges();
                if (model2 == null)
                {
                    if (Id2 != null && Sub > 0)
                    {
                        var id = Guid.Parse(Id2);
                        var model1 = dbContext.SubscriptionMethods.Find(id);

                        var mod = float.Parse(model1.Amount);
                        mod += Sub;
                        model1.Amount = mod.ToString();
                        model1.ModifiedBy = UserId;
                        dbContext.SaveChanges();

                    }

                    model.IsPaid = true;
                    model.PaperNumber1 = PaperNumber1;
                    model.PaperNumber2 = PaperNumber2;
                    model.IsPaid = true;
                    if (Date != null)
                    {
                        model.PaidDate = DateTime.Parse(Date);
                    }
                    model.PaidAmount = Amount;

                    model.ModifiedOn = DateTime.UtcNow;
                    model.ModifiedBy = UserId;
                    dbContext.SaveChanges();

                    var Revenue = new Revenue
                    {
                        Id = Guid.NewGuid(),
                        StudyYearId = model.StudentsClass.StudyYearId,
                        StudyPlaceId = model.StudentsClass.Class.StudyPlaceId,
                        RevenueTypeId = Guid.Parse("0F54C94D-B010-4360-8EB1-B93E05615065"),
                        RevenueValue = double.Parse(Amount),
                        RevenueDate = model.PaidDate,
                        Notes = "تحصيل اشتراك الطالب " + model.StudentsClass.Student.Name,
                        EmployeeId = null,
                        SubscriptionMethodId = Id
                    };
                    revenuesServices.Create(Revenue, UserId);

                    return true;

                }
                else
                {
                    return false;
                }
            }
        }

        public bool Cancel(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.SubscriptionMethods.Find(Id);
                //if (float.Parse(model.PaidAmount) < float.Parse(model.Amount))
                //{
                //    return false;
                //}
                model.Amount = model.PaidAmount;
                model.PaidAmount = null;
                model.PaidDate = null;
                model.IsPaid = false;

                model.ModifiedOn = DateTime.UtcNow;
                model.ModifiedBy = UserId;
                dbContext.SaveChanges();

                var rev = revenuesServices.GetAll().Where(x => x.SubscriptionMethodId == Id).FirstOrDefault();
                revenuesServices.Delete(rev.Id, UserId);

                return true;

            }
        }

    }
}