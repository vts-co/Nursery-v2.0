using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services
{
    public class AccountServices
    {
        public ResultDto<UserInfo> Login(string userName, string password)
        {
            var result = new ResultDto<UserInfo>();
            using (var dbContext = new almohandes_DbEntities())
            {
                if(userName==null || password==null)
                {
                    result.IsSuccess = false;
                    result.Message = "اسم المستخدم او كلمة المرور غير صحيحة";
                    return result;
                }
                var pass = Security.Encrypt(password);
                var user = dbContext.Users.Where(x => x.Username == userName && x.Password == pass&&x.IsDeleted==false).FirstOrDefault();
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Message = "اسم المستخدم او كلمة المرور غير صحيحة";
                    return result;
                }

                result.Message = "تم تسجيل الدخور بنجاح";
                result.IsSuccess = true;
                result.Result = new UserInfo()
                {
                    RoleId = (Role)user.RoleId,
                    UserId = user.Id,
                    UserName = user.Username,
                    UserScreens=user.UserScreens
                };
            }
            return result;
        }
    }
}