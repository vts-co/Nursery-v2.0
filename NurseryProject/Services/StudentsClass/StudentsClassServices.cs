using NurseryProject.Dtos.StudentsClass;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudentsClass
{
    public class StudentsClassServices
    {
        public List<StudentsClassDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var id1 = Guid.Parse("B8650DC5-A83D-4A48-B99F-B75196A1DF8C");
                var model = dbContext.StudentsClasses.Where(x => x.IsDeleted == false && x.IsCurrent == true&&x.Student.RegistrationTypeId!= id1 && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.Class.EmployeeClasses.Any(i => i.IsDeleted == false && i.EmployeeId == EmployeeId) || x.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.EmployeeId == EmployeeId) || x.Class.StudyPlace.BuildingSupervisors.Any(k => k.IsDeleted == false && k.EmployeeId == EmployeeId))).OrderBy(x => x.CreatedOn).Select(x => new StudentsClassDto
                {
                    Id = x.Id,
                    Code = x.Student.Code,
                    StudyPlaceId = x.Class.StudyPlaceId.Value,
                    StudyPlaceName = x.Class.StudyPlace.Name,
                    StudyTypeId = x.Class.Level.StudyTypeId.Value,
                    IsCurrent = x.IsCurrent.Value,
                    StudyTypeName = x.Class.Level.StudyType.Name,
                    LevelId = x.Class.LevelId.Value,
                    LevelName = x.Class.Level.Name,
                    ClassId = x.ClassId.Value,
                    ClassName = x.Class.Name,
                    StudyYearId = x.StudyYearId.Value,
                    StudyYearName = x.StudyYear.Name,
                    StudentId = x.Student.Id,
                    StudentName = x.Student.Name,
                    StudentCode = x.Student.Code,
                    StudentPhone = x.Student.Phone,
                    IsAnother = x.IsAnother.Value,
                    SubscriptionId = x.SubscriptionId.Value,
                    SubscriptionName = x.IsAnother == true ? "أخري" : x.Subscription.SubscriptionsType.Name,
                    Amount = x.IsAnother != true ? x.Subscription.Amount : "",
                    Number = x.IsAnother != true ? x.Subscription.InstallmentsNumber : "",
                    Regular = "منتظم",
                    SubscriptionMethod = x.SubscriptionMethods.Where(y => y.IsDeleted == false && y.StudentClassId == x.Id).OrderBy(y => y.OrderDisplay).Select(y => new SubscriptionMethodDto
                    {
                        Id = y.Id,
                        StudentClassId = x.Id,
                        Amount = y.Amount,
                        Date = y.Date.Value.ToString(),
                        PaidAmount = y.PaidAmount,
                        IsPaid = y.IsPaid.Value,
                        Collector=y.ModifiedBy != null?y.ModifiedBy.ToString():"",
                        Paided = y.IsPaid.Value == true ? "تم الدفع" : "لم يتم الدفع بعد",
                        PaidDate = y.PaidDate.Value.ToString()
                    }).ToList(),
                    JoiningDate = x.JoiningDate.Value.ToString(),
                    Notes = x.Notes

                }).ToList();
                foreach (var item in model)
                {
                    var total = 0.0;

                    if (item.IsAnother)
                    {
                        if (item.SubscriptionMethod.Where(y => y.IsPaid == true).Count() > 0)
                        {
                            total = float.Parse(item.SubscriptionMethod.Where(y => y.IsPaid == true).Sum(y => float.Parse(y.PaidAmount)).ToString());
                        }
                        if (item.SubscriptionMethod.Where(y => y.IsPaid == false).Count() > 0)
                        {
                            total = float.Parse(item.SubscriptionMethod.Where(y => y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString());
                        }
                        item.Amount = total.ToString();
                        item.Number = item.SubscriptionMethod.Where(y => y.StudentClassId == item.Id).ToList().Count().ToString();
                    }
                    item.StudentsClassPrevious = GetAllPrevious(item.StudentId, item.StudyTypeId);
                    item.Paid = item.SubscriptionMethod.Where(y => y.IsPaid == true).Sum(y => float.Parse(y.PaidAmount)).ToString();
                    item.Remain = item.SubscriptionMethod.Where(y => y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString();

                    foreach (var item2 in item.SubscriptionMethod)
                    {
                        if (DateTime.Parse(item2.Date).Date.AddDays(15) < DateTime.Now.Date)
                        {
                            item.Regular = "غير منتظم";
                            break;
                        }
                        if (item2.Collector != ""&& item2.IsPaid==true)
                        {
                            var id = Guid.Parse(item2.Collector);
                            var ff = dbContext.Users.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefault();
                            item2.Collector = ff.Username;
                        }
                        else 
                        {
                            item2.Collector = "";
                        }
                    }

                }
                return model;
            }
        }
        public List<StudentsClassDto> GetAllPrevious(Guid StudentId, Guid StudyTypeId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsClasses.Where(x => x.IsDeleted == false && x.Class.Level.StudyTypeId == StudyTypeId && x.IsCurrent == false && x.StudentId == StudentId).OrderBy(x => x.CreatedOn).Select(x => new StudentsClassDto
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
                    IsCurrent = x.IsCurrent.Value,
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
                        if (item.SubscriptionMethod.Where(y => y.IsPaid == true).Count() > 0)
                        {
                            total += float.Parse(item.SubscriptionMethod.Where(y => y.IsPaid == true).Sum(y => float.Parse(y.PaidAmount)).ToString());
                        }
                        if (item.SubscriptionMethod.Where(y => y.IsPaid == false).Count() > 0)
                        {
                            total += float.Parse(item.SubscriptionMethod.Where(y => y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString());
                        }
                        item.Amount = total.ToString();
                        item.Number = item.SubscriptionMethod.Where(y => y.StudentClassId == item.Id).ToList().Count().ToString();
                    }
                    item.Paid = item.SubscriptionMethod.Where(y => y.IsPaid == true).Sum(y => float.Parse(y.PaidAmount)).ToString();
                    item.Remain = item.SubscriptionMethod.Where(y => y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString();
                }

                return model;

            }
        }

        public StudentsClassDto Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsClasses.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).Select(x => new StudentsClassDto
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
                    IsCurrent = x.IsCurrent.Value,
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
                        Paided = y.IsPaid.Value == true ? "تم الدفع" : "لم يتم الدفع بعد",
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
                        total += float.Parse(model.SubscriptionMethod.Where(y => y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString());
                    }
                    model.Amount = total.ToString();
                    model.Number = model.SubscriptionMethod.Where(y => y.StudentClassId == model.Id).ToList().Count().ToString();
                }
                model.Paid = model.SubscriptionMethod.Where(y => y.IsPaid == true).Sum(y => float.Parse(y.PaidAmount)).ToString();
                model.Remain = model.SubscriptionMethod.Where(y => y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString();
                return model;
            }
        }
        public StudentsClassDto GetByStudentCurent(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsClasses.Where(x => x.IsDeleted == false && x.StudentId == Id&&x.IsCurrent==true).OrderBy(x => x.CreatedOn).Select(x => new StudentsClassDto
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
                    IsCurrent = x.IsCurrent.Value,
                    IsAnother = x.IsAnother.Value,

                    SubscriptionId = x.SubscriptionId.Value,
                    SubscriptionName = x.IsAnother == true ? "أخري" : x.Subscription.SubscriptionsType.Name,
                    Amount = x.IsAnother != true ? x.Subscription.Amount : "",
                    Number = x.IsAnother != true ? x.Subscription.InstallmentsNumber : "",

                }).FirstOrDefault();                
                return model;
            }
        }

        public List<StudentsClassDto> GetAllPrevious(Guid StudentId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsClasses.Where(x => x.IsDeleted == false && x.IsCurrent == false && x.StudentId == StudentId).OrderBy(x => x.CreatedOn).Select(x => new StudentsClassDto
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
                    IsCurrent = x.IsCurrent.Value,
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
                        if (item.SubscriptionMethod.Where(y => y.IsPaid == true).Count() > 0)
                        {
                            total += float.Parse(item.SubscriptionMethod.Where(y => y.IsPaid == true).Sum(y => float.Parse(y.PaidAmount)).ToString());
                        }
                        if (item.SubscriptionMethod.Where(y => y.IsPaid == false).Count() > 0)
                        {
                            total += float.Parse(item.SubscriptionMethod.Where(y => y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString());
                        }
                        item.Amount = total.ToString();
                        item.Number = item.SubscriptionMethod.Where(y => y.StudentClassId == item.Id).ToList().Count().ToString();
                    }
                    item.Paid = item.SubscriptionMethod.Where(y => y.IsPaid == true).Sum(y => float.Parse(y.PaidAmount)).ToString();
                    item.Remain = item.SubscriptionMethod.Where(y => y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString();
                }

                return model;

            }
        }

        public List<StudentsClassDto> GetDayMoney(string Date, string Date2)
        {
            var date = int.Parse(DateTime.Parse(Date).ToString("yyyy"));
            var date2 = int.Parse(DateTime.Parse(Date).ToString("MM"));
            var date3 = int.Parse(DateTime.Parse(Date).ToString("dd"));

            var date4 = int.Parse(DateTime.Parse(Date2).ToString("yyyy"));
            var date5 = int.Parse(DateTime.Parse(Date2).ToString("MM"));
            var date6 = int.Parse(DateTime.Parse(Date2).ToString("dd"));

            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.SubscriptionMethods.Where(x => x.IsDeleted == false && x.IsPaid == true && x.PaidDate.Value != null && x.PaidDate.Value.Year >= date && x.PaidDate.Value.Month >= date2 && x.PaidDate.Value.Day >= date3 && x.PaidDate.Value.Year <= date4 && x.PaidDate.Value.Month <= date5 && x.PaidDate.Value.Day <= date6).OrderBy(x => x.CreatedOn).Select(x => new StudentsClassDto
                {
                    Id = x.Id,
                    Code = x.StudentsClass.Student.Code,

                    StudyPlaceId = x.StudentsClass.Class.StudyPlaceId.Value,
                    StudyPlaceName = x.StudentsClass.Class.StudyPlace.Name,
                    StudyTypeId = x.StudentsClass.Class.Level.StudyTypeId.Value,

                    StudyTypeName = x.StudentsClass.Class.Level.StudyType.Name,
                    LevelId = x.StudentsClass.Class.LevelId.Value,
                    LevelName = x.StudentsClass.Class.Level.Name,
                    ClassId = x.StudentsClass.ClassId.Value,
                    ClassName = x.StudentsClass.Class.Name,
                    StudyYearId = x.StudentsClass.StudyYearId.Value,
                    StudyYearName = x.StudentsClass.StudyYear.Name,
                    StudentId = x.StudentsClass.Student.Id,
                    StudentName = x.StudentsClass.Student.Name,
                    StudentPhone = x.StudentsClass.Student.Phone,
                    IsCurrent = x.StudentsClass.IsCurrent.Value,
                    IsAnother = x.StudentsClass.IsAnother.Value,

                    SubscriptionId = x.StudentsClass.SubscriptionId.Value,
                    SubscriptionName = x.StudentsClass.IsAnother == true ? "أخري" : x.StudentsClass.Subscription.SubscriptionsType.Name,
                    Amount = x.StudentsClass.IsAnother != true ? x.StudentsClass.Subscription.Amount : "",
                    Number = x.StudentsClass.IsAnother != true ? x.StudentsClass.Subscription.InstallmentsNumber : "",
                    Paid = x.PaidAmount,
                    Collector=x.ModifiedBy != null? x.ModifiedBy.ToString():"",
                    Date = x.PaidDate.Value.ToString()

                }).ToList();
                var total = 0.0;
                foreach (var item in model)
                {
                    if (item.IsAnother)
                    {
                        if (item.SubscriptionMethod.Where(y => y.IsPaid == true).Count() > 0)
                        {
                            total += float.Parse(item.SubscriptionMethod.Where(y => y.IsPaid == true).Sum(y => float.Parse(y.PaidAmount)).ToString());
                        }
                        if (item.SubscriptionMethod.Where(y => y.IsPaid == false).Count() > 0)
                        {
                            total += float.Parse(item.SubscriptionMethod.Where(y => y.IsPaid == false).Sum(y => float.Parse(y.Amount)).ToString());
                        }
                        item.Amount = total.ToString();
                        item.Number = item.SubscriptionMethod.Where(y => y.StudentClassId == item.Id).ToList().Count().ToString();
                    }
                    if(item.Collector!="")
                    {
                        var id = Guid.Parse(item.Collector);
                        var ff = dbContext.Users.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefault();
                        item.Collector = ff.Username;
                    }
                }

                return model;
            }
        }

        public ResultDto<StudentsClassDto> UpateClassId(Guid Id, Guid ClassId, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassDto>();
                var Oldmodel = dbContext.StudentsClasses.Find(Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.ClassId = ClassId;
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }

        }
        public ResultDto<StudentsClassDto> UpateCurrentId(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassDto>();
                var Oldmodel = dbContext.StudentsClasses.Find(Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.IsCurrent = false;
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }

        }
        public ResultDto<StudentsClassDto> Create(StudentsClassDto model, Guid RegistrationTypeId, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassDto>();
                var Oldmodel = dbContext.StudentsClasses.Where(x => x.StudentId == model.StudentId && x.IsCurrent == true && x.StudyYearId == model.StudyYearId && x.Class.Level.StudyTypeId == model.StudyTypeId && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الطالب تم التحاقه بالفعل";
                    return result;
                }
                var student = dbContext.Students.Where(x => !x.IsDeleted && x.Id == model.StudentId).FirstOrDefault();
                student.RegistrationTypeId = RegistrationTypeId;
                dbContext.SaveChanges();

                var studentsClass = new Models.StudentsClass()
                {
                    Id = model.Id,
                    ClassId = model.ClassId,
                    StudentId = model.StudentId,
                    StudyYearId = model.StudyYearId,
                    SubscriptionId = model.SubscriptionId,
                    IsAnother = model.IsAnother,
                    IsCurrent = model.IsCurrent,

                    JoiningDate = DateTime.Parse(model.JoiningDate),
                    Notes = model.Notes,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = UserId,
                    IsDeleted = false,
                };
                dbContext.StudentsClasses.Add(studentsClass);
                dbContext.SaveChanges();
                if (model.SubscriptionMethod != null)
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
        public ResultDto<StudentsClassDto> Create(StudentsClassDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassDto>();
                var Oldmodel = dbContext.StudentsClasses.Where(x => x.StudentId == model.StudentId && x.IsCurrent == true && x.Class.Level.StudyTypeId == model.StudyTypeId && x.StudyYearId == model.StudyYearId && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الطالب تم التحاقه بالفعل";
                    return result;
                }

                var studentsClass = new Models.StudentsClass()
                {
                    Id = model.Id,
                    ClassId = model.ClassId,
                    StudentId = model.StudentId,
                    StudyYearId = model.StudyYearId,
                    SubscriptionId = model.SubscriptionId,
                    IsAnother = model.IsAnother,
                    IsCurrent = model.IsCurrent,

                    JoiningDate = DateTime.Parse(model.JoiningDate),
                    Notes = model.Notes,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = UserId,
                    IsDeleted = false,
                };
                dbContext.StudentsClasses.Add(studentsClass);
                dbContext.SaveChanges();
                if (model.SubscriptionMethod != null)
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

        public ResultDto<StudentsClassDto> Edit(StudentsClassDto model, Guid RegistrationTypeId, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassDto>();
                var Oldmodel2 = dbContext.StudentsClasses.Where(x => x.StudentId == model.StudentId && x.Id != model.Id && x.Class.Level.StudyTypeId == model.StudyTypeId && x.IsCurrent == true && x.StudyYearId == model.StudyYearId && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel2 != null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الطالب تم التحاقه بالفعل";
                    return result;
                }

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
                Oldmodel.IsCurrent = model.IsCurrent;

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
                var dd = DateTime.Now;
                foreach (var item in Oldmodel.SubscriptionMethods.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).ToList())
                {
                    item.Amount = model.SubscriptionMethod[i].Amount;
                    item.Date = DateTime.Parse(model.SubscriptionMethod[i].Date);
                    if (item.IsPaid == false)
                    {
                        item.IsPaid = model.SubscriptionMethod[i].IsPaid;
                        if (model.SubscriptionMethod[i].PaidDate != null && item.IsPaid == true)
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