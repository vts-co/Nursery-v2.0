using NurseryProject.Models;
using NurseryProject.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.Users
{
    public class UsersServices
    {
        public List<Page> GetAllPerants()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Pages.Where(x => x.IsDeleted == false && x.ParentId == null && x.Id != 68).OrderBy(x => x.DisplayOrder).ToList();
                return model;
            }
        }
        public List<Page> GetAllChilds()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Pages.Where(x => x.IsDeleted == false && x.ParentId != null && x.Page1.ParentId == null && x.Id != 69).OrderBy(x => x.DisplayOrder).ToList();
                return model;
            }
        }
        public List<User> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var AdminId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7");
                var model = dbContext.Users.Where(x => x.IsDeleted == false && x.Id != AdminId).OrderBy(x => x.CreatedOn).ToList();
                return model;
            }
        }
        public User Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Users.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<User> Create(User model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<User>();
                var pass = Security.Encrypt(model.Password);

                var Oldmodel = dbContext.Users.Where(x => x.Username == model.Username && x.Password == pass && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = "هذا المستخدم موجود بالفعل";
                    return result;
                }
                if (model.EmployeeId == Guid.Empty || model.EmployeeId == null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = "اختر المستخدم";
                    return result;
                }
                if (model.UserScreens == "" || model.UserScreens == null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = "اختر صلاحيات للمستخدم";
                    return result;
                }
                model.Password = pass;
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.Users.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<User> Edit(User model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<User>();
                var pass = Security.Encrypt(model.Password);

                var Oldmodel = dbContext.Users.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا المستخدم غير موجود ";
                    return result;
                }
                var Oldmodel2 = dbContext.Users.Where(x => x.Username == model.Username && x.Password == pass&&x.Id!=model.Id && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel2 != null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = "هذا المستخدم موجود بالفعل";
                    return result;
                }
                if (model.EmployeeId == Guid.Empty || model.EmployeeId == null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = "اختر المستخدم";
                    return result;
                }
                if (model.UserScreens == "" || model.UserScreens == null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = "اختر صلاحيات للمستخدم";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.Username = model.Username;
                Oldmodel.Password = pass;
                Oldmodel.UserScreens = model.UserScreens;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.RoleId = model.RoleId;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<User> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<User>();
                var Oldmodel = dbContext.Users.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا المستخدم غير موجود ";
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