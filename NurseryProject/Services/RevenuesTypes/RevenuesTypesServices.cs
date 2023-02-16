using NurseryProject.Dtos.RevenuesTypes;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.RevenuesTypes
{
    public class RevenuesTypesServices
    {
        public List<RevenuesTypesDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.RevenuesTypes.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new RevenuesTypesDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.ParentId == null ? Guid.Empty : x.ParentId.Value,
                    ParentName = x.ParentId == null ? "" : x.RevenuesType1.Name,
                    Notes = x.Notes
                }).ToList();
                return model;
            }
        }
        public RevenuesType Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.RevenuesTypes.Where(x => x.IsDeleted == false&&x.Id==Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<RevenuesType> Create(RevenuesType model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<RevenuesType>();
                var Oldmodel = dbContext.RevenuesTypes.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "نوع الايراد موجود بالفعل";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.RevenuesTypes.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<RevenuesType> Edit(RevenuesType model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<RevenuesType>();
                var Oldmodel = dbContext.RevenuesTypes.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "نوع الايراد غير موجود ";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.Name = model.Name;
                Oldmodel.Notes = model.Notes;
                Oldmodel.ParentId = model.ParentId;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<RevenuesType> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<RevenuesType>();
                var Oldmodel = dbContext.RevenuesTypes.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "نوع الايراد غير موجود ";
                    return result;
                }
                if (Id == Guid.Parse("73D54EA9-BA7D-4EC3-90EC-C7499C859FB8") || Id == Guid.Parse("0F54C94D-B010-4360-8EB1-B93E05615065"))
                {
                    result.IsSuccess = false;
                    result.Message = "نوع الايراد لا يمكن حذفها ";
                    return result;
                }
                if (Oldmodel.Revenues.Any(y => y.IsDeleted == false) || Oldmodel.RevenuesTypes1.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = "نوع الايراد لا يمكن حذفها ";
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