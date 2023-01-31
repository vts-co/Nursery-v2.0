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
                var model = dbContext.Revenues.Where(x => x.IsDeleted == false&&x.StudyPlace.IsDeleted==false&&x.StudyYear.IsDeleted==false&&x.RevenuesType.IsDeleted==false&&x.Employee.IsDeleted==false).OrderBy(x => x.CreatedOn).Select(x => new RevenuesDto
                {
                    Id = x.Id,
                    StudyPlaceId = x.StudyPlaceId.Value,
                    StudyPlaceName = x.StudyPlace.Name,
                    StudyYearId = x.StudyYearId.Value,
                    StudyYearName = x.StudyYear.Name,
                    RevenueTypeId =x.RevenueTypeId.Value,
                    RevenueTypeName=x.RevenuesType.Name,
                    EmployeeId = x.EmployeeId.Value,
                    EmployeeName = x.Employee.Name,
                    Value = x.RevenueValue,
                    Date=x.RevenueDate.Value,
                    Notes = x.Notes,
                    
                }).ToList();
                return model;
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