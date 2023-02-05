using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.Settings
{
    public class SettingsServices
    {
        public Setting GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Settings.FirstOrDefault();
                return model;
            }
        }

        public ResultDto<Setting> Create(Setting model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Setting>();
                var Oldmodel = dbContext.Settings.FirstOrDefault();
                if (Oldmodel == null)
                {
                    model.CreatedOn = DateTime.UtcNow;
                    model.CreatedBy = UserId;
                    model.IsDeleted = false;
                    dbContext.Settings.Add(model);
                    dbContext.SaveChanges();
                }
                else
                {
                    Oldmodel.ModifiedOn = DateTime.UtcNow;
                    Oldmodel.ModifiedBy = UserId;
                    Oldmodel.Title = model.Title;
                    Oldmodel.Logo = model.Logo;
                    dbContext.SaveChanges();

                }
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Setting> Edit(Setting model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Setting>();
                
                var Oldmodel = dbContext.Settings.FirstOrDefault();
                if (Oldmodel == null)
                {
                    if (model.Logo == null || model.Title == null)
                    {
                        result.IsSuccess = false;
                        result.Message = "الرجاء ادخال البيانات بشكل صحيح";
                        return result;
                    }
                    model.CreatedOn = DateTime.UtcNow;
                    model.CreatedBy = UserId;
                    model.IsDeleted = false;
                    dbContext.Settings.Add(model);
                    dbContext.SaveChanges();
                }
                else
                {
                    Oldmodel.ModifiedOn = DateTime.UtcNow;
                    Oldmodel.ModifiedBy = UserId;
                    if (model.Logo != null)
                    {
                        Oldmodel.Logo = model.Logo;
                    }
                    if (model.Title != null)
                    {
                        Oldmodel.Title = model.Title;
                    }
                    dbContext.SaveChanges();

                }
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Setting> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Setting>();
                var Oldmodel = dbContext.Settings.Find(Id);

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