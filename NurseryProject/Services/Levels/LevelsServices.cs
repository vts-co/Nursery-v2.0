using NurseryProject.Dtos.Levels;
using NurseryProject.Models;
using NurseryProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Levels.Services
{
    public class LevelsServices
    {
        public List<LevelsDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Levels.Where(x => x.IsDeleted == false).OrderBy(x => x.DisplayOrder).Select(x => new LevelsDto
                {
                    Id = x.Id,
                    StudyTypeId = x.StudyTypeId.Value,
                    StudyTypeName = x.StudyType.Name,
                    DisplayOrder=x.DisplayOrder.Value,
                    Name = x.Name,
                    Notes = x.Notes
                }).ToList();
                return model;
            }
        }
        public Level Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Levels.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Level> Create(Level model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Level>();
                var Oldmodel = dbContext.Levels.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "هذا الصف موجود بالفعل";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.Levels.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Level> Edit(Level model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Level>();
                var Oldmodel = dbContext.Levels.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الصف موجود بالفعل";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.StudyTypeId = model.StudyTypeId;
                Oldmodel.Name = model.Name;
                Oldmodel.Notes = model.Notes;
                Oldmodel.DisplayOrder = model.DisplayOrder;
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Level> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Level>();
                var Oldmodel = dbContext.Levels.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الصف غير موجود ";
                    return result;
                }
                if (Oldmodel.Classes.Any(y => y.IsDeleted == false) || Oldmodel.Subscriptions.Any(y => y.IsDeleted == false) || Oldmodel.Subjects.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = "هذه الحالة بها طلاب لا يمكن حذفها ";
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