using NurseryProject.Dtos.Subjects;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.Subjects
{
    public class SubjectsServices
    {
        public List<SubjectsDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Subjects.Where(x => x.IsDeleted == false&&x.Level.IsDeleted==false).OrderBy(x => x.CreatedOn).Select(x => new SubjectsDto
                {
                    Id = x.Id,
                    StudyTypeId = x.Level.StudyType.Id,
                    StudyTypeName = x.Level.StudyType.Name,
                    LevelId = x.LevelId.Value,
                    LevelName = x.Level.Name,
                    Name = x.Name,
                    Notes = x.Notes
                }).ToList();
                return model;
            }
        }
        public Subject Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Subjects.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Subject> Create(Subject model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Subject>();
                var Oldmodel = dbContext.Subjects.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = "هذه المادة موجودة بالفعل";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.Subjects.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Subject> Edit(Subject model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Subject>();
                var Oldmodel = dbContext.Subjects.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذه المادة موجودة بالفعل";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.LevelId = model.LevelId;
                Oldmodel.Name = model.Name;
                Oldmodel.Notes = model.Notes;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Subject> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Subject>();
                var Oldmodel = dbContext.Subjects.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذه المادة غير موجودة ";
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