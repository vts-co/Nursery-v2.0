using NurseryProject.Dtos.Expenses;
using NurseryProject.Dtos.Revenues;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.Revenues
{
    public class RevenuesServices
    {
        public List<RevenuesDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Revenues.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new RevenuesDto
                {
                    Id = x.Id,
                    StudyPlaceId = x.StudyPlaceId.Value,
                    StudyPlaceName = x.StudyPlace.Name,
                    ClassId = x.ClassId == null ? Guid.Empty : x.Class.Id,
                    ClassName = x.ClassId == null ? "" : x.Class.Name,
                    StudyYearId = x.StudyYearId.Value,
                    StudyYearName = x.StudyYear.Name,
                    RevenueTypeParentId=x.RevenuesType.ParentId.Value,
                    RevenueTypeParentName=x.RevenuesType.RevenuesType1.Name,
                    RevenueTypeId =x.RevenueTypeId.Value,
                    RevenueTypeName=x.RevenuesType.Name,
                    EmployeeId = x.EmployeeId == null ? Guid.Empty : x.EmployeeId.Value,
                    EmployeeName = x.EmployeeId == null ? "" : x.Employee.Name,
                    Value = x.RevenueValue,
                    Date=x.RevenueDate.Value,
                    Notes = x.Notes,
                    PaperNumber1= x.PaperNumber1,
                    PaperNumber2=x.PaperNumber2,
                    SubscriptionMethodId = x.SubscriptionMethodId==null?Guid.Empty:x.SubscriptionMethodId.Value
                }).ToList();
                return model;
            }
        }


        public List<ExpensesCollectedDto> GetAllCollected(string date, string date2, Guid? RevenueTypeParentId = null, Guid? RevenueTypeId = null, Guid? StudyPlaceId = null, Guid? ClassId = null)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Revenues.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new RevenuesDto
                {
                    Id = x.Id,
                    StudyPlaceId = x.StudyPlaceId.Value,
                    StudyPlaceName = x.StudyPlace.Name,
                    ClassId = x.ClassId == null ? Guid.Empty : x.Class.Id,
                    ClassName = x.ClassId == null ? "" : x.Class.Name,
                    StudyYearId = x.StudyYearId.Value,
                    StudyYearName = x.StudyYear.Name,
                    RevenueTypeParentId = x.RevenuesType.ParentId.Value,
                    RevenueTypeParentName = x.RevenuesType.RevenuesType1.Name,
                    RevenueTypeId = x.RevenueTypeId.Value,
                    RevenueTypeName = x.RevenuesType.Name,
                    EmployeeId = x.EmployeeId == null ? Guid.Empty : x.EmployeeId.Value,
                    EmployeeName = x.EmployeeId == null ? "" : x.Employee.Name,
                    Value = x.RevenueValue,
                    Date = x.RevenueDate.Value,
                    Notes = x.Notes,
                    SubscriptionMethodId = x.SubscriptionMethodId == null ? Guid.Empty : x.SubscriptionMethodId.Value
                }).ToList();
                if (date != "" && date != null)
                {
                    var mon = DateTime.Parse(date).Year;
                    var mon2 = DateTime.Parse(date).Month;
                    var mon3 = DateTime.Parse(date).Day;

                    model = model.Where(x => x.Date.Year >= mon && x.Date.Month >= mon2 && x.Date.Day >= mon3).ToList();
                }
                if (date2 != "" && date2 != null)
                {
                    var mon = DateTime.Parse(date2).Year;
                    var mon2 = DateTime.Parse(date2).Month;
                    var mon3 = DateTime.Parse(date2).Day;

                    model = model.Where(x => x.Date.Year <= mon && x.Date.Month <= mon2 && x.Date.Day <= mon3).ToList();
                }
                if (StudyPlaceId != null)
                {
                    model = model.Where(x => x.StudyPlaceId == StudyPlaceId).ToList();

                
                }
          
                if (ClassId != null)
                {
                    model = model.Where(x => x.ClassId == ClassId).ToList();

                }
              
                if (RevenueTypeParentId != null)
                {
                    model = model.Where(x => x.RevenueTypeParentId == RevenueTypeParentId).ToList();
               }
               
                if (RevenueTypeId != null)
                {
                    model = model.Where(x => x.RevenueTypeId == RevenueTypeId).ToList();
                }
                var groupedData = model.GroupBy(x => x.RevenueTypeParentId);
                var dailySubscriptionMethods = groupedData.Select(group => new ExpensesCollectedDto
                {
                    ExpenseName = group.First().RevenueTypeParentName,
                    NoteName = group.First().RevenueTypeName ,
                    Amount = group.Sum(x => x.Value),
                  

                }).ToList();

                return dailySubscriptionMethods;


            }
        }

        public Revenue Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Revenues.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Revenue> Create(Revenue model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Revenue>();
                
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.Revenues.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Revenue> Edit(Revenue model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Revenue>();
                var Oldmodel = dbContext.Revenues.Find(model.Id);
               
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.StudyPlaceId = model.StudyPlaceId;
                Oldmodel.StudyYearId = model.StudyYearId;
                Oldmodel.RevenueTypeId = model.RevenueTypeId;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.RevenueValue = model.RevenueValue;
                Oldmodel.RevenueDate = model.RevenueDate;
                Oldmodel.Notes = model.Notes;
                Oldmodel.ClassId = model.ClassId;
                Oldmodel.PaperNumber1 = model.PaperNumber1;
                Oldmodel.PaperNumber2 = model.PaperNumber2;
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Revenue> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Revenue>();
                var Oldmodel = dbContext.Revenues.Find(Id);
               
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