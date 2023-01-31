using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudyTypes
{
    public class StudyTypesServices
    {
        public List<StudyType> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudyTypes.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).ToList();
                return model;
            }
        }

        public ResultDto<StudyType> Create(StudyType model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudyType>();
                var Oldmodel = dbContext.StudyTypes.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "هذا النوع موجود بالفعل";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.StudyTypes.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudyType> Edit(StudyType model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudyType>();
                var Oldmodel = dbContext.StudyTypes.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا النوع غير موجود ";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.Name = model.Name;
                Oldmodel.Notes = model.Notes;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudyType> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudyType>();
                var Oldmodel = dbContext.StudyTypes.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا النوع غير موجود ";
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