using NurseryProject.Models;
using NurseryProject.Utilities;
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

                    var userId = Guid.Parse("7C9E6679-7425-40DE-944B-E07FC1F90AE7");
                    var pass = Security.Encrypt("elmohandes@123");
                    var user = new User
                    {
                        Id = userId,
                        Username = "Admin",
                        Password = pass,
                        RoleId = 1,
                        UserScreens = ",0,2,4,5,6,7,8,10,11,12,13,14,15,16,18,19,21,22,24,25,27,28,29,30,32,33,34,35,36,37,39,40,42,43,45,46,47,48,49,50,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,69,70,71,72,73,74,75,76,",
                        CreatedOn = DateTime.UtcNow
                    };
                    db.Users.Add(user);
                    db.SaveChanges();

                    var ExpenId = Guid.Parse("A41366FB-D326-4723-A630-F81E38D5ED23");
                    var Expen = new ExpensesType
                    {
                        Id = ExpenId,
                        Name = "مرتبات",
                        CreatedOn = DateTime.UtcNow
                    };
                    db.ExpensesTypes.Add(Expen);
                    db.SaveChanges();

                    var ExpenId2 = Guid.Parse("20A6A59E-088E-4E6A-AA27-8A15F051B1DE");
                    var Expen2 = new ExpensesType
                    {
                        Id = ExpenId2,
                        ParentId= ExpenId,
                        Name = "اجور و رواتب",
                        CreatedOn = DateTime.UtcNow
                    };
                    db.ExpensesTypes.Add(Expen2);
                    db.SaveChanges();

                    var RevenId = Guid.Parse("73D54EA9-BA7D-4EC3-90EC-C7499C859FB8");
                    var Reven = new RevenuesType
                    {
                        Id = RevenId,
                        Name = "تحصيلات اشتركات",
                        CreatedOn = DateTime.UtcNow
                    };
                    db.RevenuesTypes.Add(Reven);
                    db.SaveChanges();

                    var RevenId2 = Guid.Parse("0F54C94D-B010-4360-8EB1-B93E05615065");
                    var Reven2 = new RevenuesType
                    {
                        Id = RevenId2,
                        ParentId = RevenId,
                        Name = "اشتراكات الحضانة",
                        CreatedOn = DateTime.UtcNow
                    };
                    db.RevenuesTypes.Add(Reven2);
                    db.SaveChanges();

                    var RegId = Guid.Parse("E31AC343-47DA-4DFE-8970-E1719DEEC869");
                    var Regs = new RegistrationType
                    {
                        Id = RegId,
                        Name = "مستجد",
                        CreatedOn = DateTime.UtcNow
                    };
                    db.RegistrationTypes.Add(Regs);
                    db.SaveChanges();

                    var RegId2 = Guid.Parse("5CF58B9E-D1EB-41D8-9B11-4CCC120648A3");
                    var Regs2 = new RegistrationType
                    {
                        Id = RegId2,
                        Name = "معفي",
                        CreatedOn = DateTime.UtcNow
                    };
                    db.RegistrationTypes.Add(Regs2);
                    db.SaveChanges();

                    var RegId3 = Guid.Parse("B8650DC5-A83D-4A48-B99F-B75196A1DF8C");
                    var Regs3 = new RegistrationType
                    {
                        Id = RegId3,
                        Name = "منقطع",
                        CreatedOn = DateTime.UtcNow
                    };
                    db.RegistrationTypes.Add(Regs3);
                    db.SaveChanges();


                    var SettingId = Guid.Parse("D3D28F6D-0B7E-4FE7-8EAB-DA0B3FC3ECB2");
                    var Setting = new Setting
                    {
                        Id = SettingId,
                        Title = "حضانة المهندس",
                        ReportHeader= "<p style='text-align: center;'>ايصال استلام نقدية</p>",
                        ReportFooter="<p style='text-align: center;'>جميع الحقوق محفوظة 2023</p>",
                        Logo= "/Uploads/Settings/d3d28f6d-0b7e-4fe7-8eab-da0b3fc3ecb2/d3d28f6d-0b7e-4fe7-8eab-da0b3fc3ecb2.jpg",
                        CreatedOn = DateTime.UtcNow
                    };
                    db.Settings.Add(Setting);
                    db.SaveChanges();
                }
            }
        }
    }
}