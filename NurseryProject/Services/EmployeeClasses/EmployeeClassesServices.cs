using NurseryProject.Dtos.EmployeeClasses;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeeClasses
{
    public class EmployeeClassesServices
    {
        public List<EmployeeClassesDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeeClasses.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new EmployeeClassesDto
                {
                    Id = x.Id,
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
                    EmployeeId = x.EmployeeId.Value,
                    EmployeeName = x.Employee.Name,
                    SubjectId = x.SubjectId.Value,
                    SubjecName = x.Subject.Name,

                }).ToList();
                return model;
            }
        }
        public EmployeeClass Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeeClasses.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<EmployeeClass> Create(EmployeeClass model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeClass>();

                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.EmployeeClasses.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeeClass> Edit(EmployeeClass model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeClass>();
                var Oldmodel = dbContext.EmployeeClasses.Find(model.Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.ClassId = model.ClassId;
                Oldmodel.SubjectId = model.SubjectId;
                Oldmodel.StudyYearId = model.StudyYearId;
                Oldmodel.EmployeeId = model.EmployeeId;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeeClass> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeClass>();
                var Oldmodel = dbContext.EmployeeClasses.Find(Id);

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