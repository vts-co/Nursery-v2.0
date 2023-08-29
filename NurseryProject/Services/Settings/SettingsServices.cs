using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                    Oldmodel.ReportHeader = model.ReportHeader;
                    Oldmodel.ReportFooter = model.ReportFooter;
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
                    if (model.ReportFooter == null || model.Title == null || model.ReportHeader == null || model.ReportHeader == null)
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
                    if (model.ReportHeader != null)
                    {
                        Oldmodel.ReportHeader = model.ReportHeader;
                    }
                    if (model.ReportFooter != null)
                    {
                        Oldmodel.ReportFooter = model.ReportFooter;
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

        public List<HomePage> GetAllHomePages()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.HomePages.OrderBy(x => x.DisplayOrder).ToList();
                return model;
            }
        }
        public List<Page> GetAllPages()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Pages.Where(x => x.IsDeleted == false).OrderBy(x => x.DisplayOrder).ToList();
                return model;
            }
        }
        public Page GetPage(int Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Pages.Where(x => x.IsDeleted == false && x.Id == Id).FirstOrDefault();
                return model;
            }
        }
        public void BackUp(string dbPath)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                using (var db = new almohandes_DbEntities())
                {
                    var cmd = String.Format("BACKUP DATABASE almohandes_Db TO DISK = '"+ dbPath + "'WITH FORMAT,MEDIANAME = 'C_SQLServerBackups',NAME = 'Full Backup of MyDB'; "
                            , "almohandes_Db", dbPath);
                    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);

                }
            }
        }

        public void Restore(string dbPath)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                using (var db = new almohandes_DbEntities())
                {
                    var cmd = String.Format("restore DATABASE almohandes_Db from DISK = '" + dbPath + "'WITH FORMAT,MEDIANAME = 'C_SQLServerBackups',NAME = 'Full Backup of MyDB'; "
                            , "almohandes_Db", dbPath);
                    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);

                }
            }
        }

        public void DeleteAll(string dbPath)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                using (var db = new almohandes_DbEntities())
                {
                    var cmd = String.Format("restore DATABASE almohandes_Db from DISK = '" + dbPath + "'WITH FORMAT,MEDIANAME = 'C_SQLServerBackups',NAME = 'Full Backup of MyDB'; "
                            , "almohandes_Db", dbPath);
                    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, cmd);

                }
            }
        }
    }
}