using NurseryProject.Dtos.StudentsClass;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudentsClass
{
    public class StudentsClassServices
    {
        public List<StudentsClassDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsClasses.Where(x => x.IsDeleted == false &&x.Student.IsDeleted==false&&(x.Subscription.IsDeleted==false|| x.SubscriptionId==null) &&x.StudyYear.IsDeleted==false&&x.Class.IsDeleted==false).OrderBy(x => x.CreatedOn).Select(x => new StudentsClassDto
                {
                    Id = x.Id,
                    Code = x.Student.Code,
                    StudyPlaceId = x.Class.StudyPlaceId.Value,
                    StudyPlaceName = x.Class.StudyPlace.Name,
                    StudyTypeId = x.Class.Level.StudyTypeId.Value,

                    StudyTypeName = x.Class.Level.StudyType.Name,
                    LevelId = x.Class.LevelId.Value,
                    LevelName = x.Class.Level.Name,
                    ClassId = x.ClassId.Value,
                    ClassName = x.Class.Name,
                    StudyYearId = x.StudyYearId.Value,
                    StudyYearName = x.StudyYear.Name,
                    StudentId = x.Student.Id,
                    StudentName = x.Student.Name ,
                    StudentPhone=x.Student.Phone,
                    IsAnother = x.IsAnother.Value,
                    SubscriptionId = x.SubscriptionId.Value,
                    SubscriptionName = x.IsAnother == true ? "أخري" : x.Subscription.SubscriptionsType.Name,
                    Amount = x.IsAnother != true ? x.Subscription.Amount : "",
                    Number = x.IsAnother != true ? x.Subscription.InstallmentsNumber : "",
                    SubscriptionMethod = x.SubscriptionMethods.Where(y => y.IsDeleted == false && y.StudentClassId == x.Id).OrderBy(y => y.OrderDisplay).Select(y => new SubscriptionMethodDto
                    {
                        Id = y.Id,
                        StudentClassId = x.Id,
                        Amount = y.Amount,
                        Date = y.Date.Value.ToString(),
                        PaidAmount=y.PaidAmount,
                        IsPaid = y.IsPaid.Value,
                        Paided = y.IsPaid.Value == true ? "تم الدفع" : "لم يتم الدفع بعد",
                        PaidDate = y.PaidDate.Value.ToString()
                    }).ToList(),
                    JoiningDate = x.JoiningDate.Value.ToString(),
                    Notes = x.Notes

                }).ToList();
                var total = 0.0;
                foreach (var item in model)
                {
                    if (item.IsAnother)
                    {
                        if (item.SubscriptionMethod.Where(y=>y.IsPaid == true).Count()>0)
                        {
                            total += float.Parse(item.SubscriptionMethod.Where(y => y.IsPaid == true).Sum(y => float.Parse(y.PaidAmount)).ToString());
                        }
                        if (item.SubscriptionMethod.Where(y => y.IsPaid == false).Count() > 0)
                        {
                            total += float.Parse(item.SubscriptionMethod.Where(y =>y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString());
                        }
                        item.Amount = total.ToString();
                        item.Number = item.SubscriptionMethod.Where(y => y.StudentClassId == item.Id).ToList().Count().ToString();
                    }
                }
                return model;
            }
        }
        public StudentsClassDto Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsClasses.Where(x => x.IsDeleted == false&&x.Id==Id && x.Student.IsDeleted == false && (x.Subscription.IsDeleted == false || x.SubscriptionId == null) && x.StudyYear.IsDeleted == false && x.Class.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new StudentsClassDto
                {
                    Id = x.Id,
                    Code = x.Student.Code,

                    StudyPlaceId = x.Class.StudyPlaceId.Value,
                    StudyPlaceName = x.Class.StudyPlace.Name,
                    StudyTypeId = x.Class.Level.StudyTypeId.Value,

                    StudyTypeName = x.Class.Level.StudyType.Name,
                    LevelId = x.Class.LevelId.Value,
                    LevelName = x.Class.Level.Name,
                    ClassId = x.ClassId.Value,
                    ClassName = x.Class.Name,
                    StudyYearId = x.StudyYearId.Value,
                    StudyYearName = x.StudyYear.Name,
                    StudentId = x.Student.Id,
                    StudentName = x.Student.Name,
                    StudentPhone = x.Student.Phone,

                    IsAnother = x.IsAnother.Value,

                    SubscriptionId = x.SubscriptionId.Value,
                    SubscriptionName = x.IsAnother == true ? "أخري" : x.Subscription.SubscriptionsType.Name,
                    Amount = x.IsAnother != true ? x.Subscription.Amount : "",
                    Number = x.IsAnother != true ? x.Subscription.InstallmentsNumber : "",

                    SubscriptionMethod = x.SubscriptionMethods.Where(y => y.IsDeleted == false && y.StudentClassId == x.Id).OrderBy(y => y.OrderDisplay).Select(y => new SubscriptionMethodDto
                    {
                        Id = y.Id,
                        StudentClassId = x.Id,
                        Amount = y.Amount,
                        Date = y.Date.Value.ToString(),
                        PaidAmount = y.PaidAmount,
                        IsPaid = y.IsPaid.Value,
                        Paided= y.IsPaid.Value == true ? "تم الدفع" : "لم يتم الدفع بعد",
                        PaidDate = y.PaidDate.Value.ToString()
                    }).ToList(),
                    JoiningDate = x.JoiningDate.Value.ToString(),
                    Notes = x.Notes

                }).FirstOrDefault();
                var total = 0.0;
                if (model.IsAnother)
                {
                    if (model.SubscriptionMethod.Where(y => y.IsPaid == true).Count() > 0)
                    {
                        total += float.Parse(model.SubscriptionMethod.Where(y => y.IsPaid == true).Sum(y => float.Parse(y.PaidAmount)).ToString());
                    }
                    if (model.SubscriptionMethod.Where(y => y.IsPaid == false).Count() > 0)
                    {
                        total += float.Parse(model.SubscriptionMethod.Where(y =>y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString());
                    }
                    model.Amount = total.ToString();
                    model.Number = model.SubscriptionMethod.Where(y => y.StudentClassId == model.Id).ToList().Count().ToString();
                }

                return model;
            }
        }
        public ResultDto<StudentsClassDto> Create(StudentsClassDto model, Guid RegistrationTypeId, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassDto>();
                var Oldmodel = dbContext.StudentsClasses.Where(x=>x.StudentId==model.StudentId &&x.StudyYearId==model.StudyYearId && x.IsDeleted == false ).FirstOrDefault();
                if(Oldmodel!=null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الطالب تم التحاقه بالفعل";
                    return result;
                }
                var student = dbContext.Students.Find(model.StudentId);
                student.RegistrationTypeId = RegistrationTypeId;
                dbContext.SaveChanges();

                var studentsClass = new Models.StudentsClass()
                {
                    Id = model.Id,
                    ClassId = model.ClassId,
                    StudentId = model.StudentId,
                    StudyYearId = model.StudyYearId,
                    SubscriptionId = model.SubscriptionId,
                    IsAnother=model.IsAnother,
                    JoiningDate = DateTime.Parse(model.JoiningDate),
                    Notes = model.Notes,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = UserId,
                    IsDeleted = false,
                };
                dbContext.StudentsClasses.Add(studentsClass);
                dbContext.SaveChanges();
                if(model.SubscriptionMethod!=null)
                {
                    var i = 1;
                    foreach (var item in model.SubscriptionMethod)
                    {
                        var subscriptionMethod = new SubscriptionMethod()
                        {
                            Id = Guid.NewGuid(),
                            StudentClassId = model.Id,
                            Amount = item.Amount,
                            Date = DateTime.Parse(item.Date),
                            OrderDisplay = i,
                            PaidAmount = item.PaidAmount,
                            IsPaid = false,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = UserId,
                            IsDeleted = false,
                        };
                        dbContext.SubscriptionMethods.Add(subscriptionMethod);
                        dbContext.SaveChanges();
                        i++;
                    }
                }
                
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudentsClassDto> Edit(StudentsClassDto model,Guid RegistrationTypeId, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassDto>();
                var Oldmodel = dbContext.StudentsClasses.Find(model.Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.ClassId = model.ClassId;
                Oldmodel.IsAnother = model.IsAnother;
                Oldmodel.StudyYearId = model.StudyYearId;
                Oldmodel.SubscriptionId = model.SubscriptionId;
                Oldmodel.StudentId = model.StudentId;
                Oldmodel.JoiningDate = DateTime.Parse(model.JoiningDate);
                Oldmodel.Notes = model.Notes;
                Oldmodel.Student.RegistrationTypeId = RegistrationTypeId;

                var subscriptionMethod1 = dbContext.SubscriptionMethods.Where(x => x.StudentClassId == model.Id && x.IsDeleted == false).ToList();
                if (subscriptionMethod1.Count() > 0)
                {
                    if (subscriptionMethod1.Any(x => x.IsPaid.Value))
                    {
                        result.IsSuccess = false;
                        result.Message = "لا يمكن تعديل الاشتراك لان تم دفع اقساط منه ";
                        return result;
                    }
                    foreach (var item in subscriptionMethod1)
                    {
                        item.IsDeleted = true;
                        item.DeletedBy = UserId;
                        item.DeletedOn = DateTime.UtcNow;
                        dbContext.SaveChanges();
                    }
                }
                var i = 1;

                foreach (var item in model.SubscriptionMethod)
                {
                    var subscriptionMethod = new SubscriptionMethod();
                    {
                        subscriptionMethod.Id = Guid.NewGuid();
                        subscriptionMethod.StudentClassId = model.Id;
                        subscriptionMethod.OrderDisplay = i;
                        subscriptionMethod.Amount = item.Amount;
                        subscriptionMethod.PaidAmount = item.PaidAmount;
                        subscriptionMethod.IsPaid = item.IsPaid;
                        subscriptionMethod.Date = DateTime.Parse(item.Date);
                        subscriptionMethod.CreatedOn = DateTime.UtcNow;
                        subscriptionMethod.CreatedBy = UserId;
                        subscriptionMethod.IsDeleted = false;
                    };
                    dbContext.SubscriptionMethods.Add(subscriptionMethod);
                    dbContext.SaveChanges();
                    i++;
                }

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudentsClassDto> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassDto>();
                var Oldmodel = dbContext.StudentsClasses.Find(Id);
                var subscriptionMethod1 = dbContext.SubscriptionMethods.Where(x => x.StudentClassId == Id && x.IsDeleted == false).ToList();
                if (subscriptionMethod1.Count() > 0)
                {
                    if (subscriptionMethod1.Any(x => x.IsPaid.Value))
                    {
                        result.IsSuccess = false;
                        result.Message = "لا يمكن حذف الاشتراك لان تم دفع اقساط منه ";
                        return result;
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
        public ResultDto<StudentsClassDto> Collect(StudentsClassDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassDto>();
                var Oldmodel = dbContext.StudentsClasses.Find(model.Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.ClassId = model.ClassId;
                Oldmodel.IsAnother = model.IsAnother;
                Oldmodel.StudyYearId = model.StudyYearId;
                Oldmodel.SubscriptionId = model.SubscriptionId;
                Oldmodel.StudentId = model.StudentId;
                Oldmodel.JoiningDate = DateTime.Parse(model.JoiningDate);
                Oldmodel.Notes = model.Notes;


                var i = 0;
                var dd =DateTime.Now;
                foreach (var item in Oldmodel.SubscriptionMethods.Where(x => x.IsDeleted == false).OrderBy(x=>x.CreatedOn).ToList())
                {

                    item.Amount = model.SubscriptionMethod[i].Amount;
                    item.Date = DateTime.Parse(model.SubscriptionMethod[i].Date);
                    if(item.IsPaid==false)
                    {
                        item.IsPaid = model.SubscriptionMethod[i].IsPaid;
                        if(model.SubscriptionMethod[i].PaidDate!=null && item.IsPaid==true)
                        {
                            DateTime.TryParse(model.SubscriptionMethod[i].PaidDate, out dd);
                            item.PaidDate = dd;
                        }
                        
                    }
                   
                    item.ModifiedOn = DateTime.UtcNow;
                    item.ModifiedBy = UserId;

                    dbContext.SaveChanges();
                    i++;
                }

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
    }
}