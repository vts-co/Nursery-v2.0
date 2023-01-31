using NurseryProject.Dtos.StudyClasses;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudyClasses
{
    public class StudyClassesServices
    {
        public List<StudyClassesDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudyClasses.Where(x => x.IsDeleted == false&&x.StudyYear.IsDeleted==false).OrderBy(x => x.CreatedOn).Select(x => new StudyClassesDto
                {
                    Id = x.Id,
                    StudyYearId = x.StudyYearId.Value,
                    StudyYearName = x.StudyYear.Name,
                    Name = x.Name,
                    Notes = x.Notes
                }).ToList();
                return model;
            }
        }
        public StudyClass Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudyClasses.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<StudyClass> Create(StudyClass model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudyClass>();
                var Oldmodel = dbContext.StudyClasses.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "هذا الفصل موجود بالفعل";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.StudyClasses.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudyClass> Edit(StudyClass model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudyClass>();
                var Oldmodel = dbContext.StudyClasses.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الفصل موجود بالفعل";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.StudyYearId = model.StudyYearId;
                Oldmodel.Name = model.Name;
                Oldmodel.Notes = model.Notes;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudyClass> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudyClass>();
                var Oldmodel = dbContext.StudyClasses.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الفصل غير موجود ";
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