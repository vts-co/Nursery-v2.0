using NurseryProject.Enums;
using NurseryProject.Services.Settings;
using NurseryProject.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NurseryProject.Authorization
{
    public class Authorized : ActionFilterAttribute, IExceptionFilter
    {
        public Role Role { get; set; } = 0;
        public Guid UserId { get; set; }
        public Guid EmployeeId { get; set; }

        public string[] UserScreens = null;
        public string ScreenId = null;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SettingsServices settingsServices = new SettingsServices();
            string search = "," + ScreenId + ",";
            UsersServices usersServices = new UsersServices();
            var controller = filterContext.Controller as Controller;
            if (controller != null)
            {
                VTSAuth auth = new VTSAuth() { CookieValues = new UserInfo { } };
                var load = auth.LoadDataFromCookies();
                var user = usersServices.Get(auth.CookieValues.UserId);

                if (!load || user == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "SignIn", returnUrl = filterContext.HttpContext.Request.Url.ToString() }));
                    return;
                }
                //else if ((auth.CookieValues.RoleId & Role) == 0)
                //{
                //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {  controller = "Account", action = "SignIn", returnUrl = filterContext.HttpContext.Request.Url.ToString() }));

                //}
                else if (ScreenId == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "SignIn", returnUrl = filterContext.HttpContext.Request.Url.ToString() }));

                }
                else if (user.UserScreens == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "SignIn", returnUrl = filterContext.HttpContext.Request.Url.ToString() }));

                }
                else if (!user.UserScreens.Contains(search))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "SignIn", returnUrl = filterContext.HttpContext.Request.Url.ToString() }));
                }
                filterContext.Controller.TempData["UserInfo"] = auth.CookieValues;
                filterContext.Controller.TempData["UserId"] = auth.CookieValues.UserId;
                filterContext.Controller.TempData["EmployeeId"] = auth.CookieValues.EmployeeId;
                filterContext.Controller.TempData["RoleId"] = auth.CookieValues.RoleId;

                filterContext.Controller.ViewBag.UserScreens = user.UserScreens;
                filterContext.Controller.ViewBag.UserName = user.Username;
                if (user.EmployeeId==null)
                    filterContext.Controller.ViewBag.EmployeeName = "Admin";
                else
                    filterContext.Controller.ViewBag.EmployeeName = user.Employee.Name;

                var setting = settingsServices.GetAll();

                filterContext.Controller.TempData["SettingLogo"] = setting.Logo;
                filterContext.Controller.TempData["SettingTitle"] = setting.Title;
                if (auth.CookieValues.UserScreens != null)
                {
                    string[] strArray = auth.CookieValues.UserScreens.Split(',');
                    UserScreens = strArray;

                }
                
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "SignIn", returnUrl = filterContext.HttpContext.Request.Url.ToString() }));
                return;
            }
        }

        public void OnException(ExceptionContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {  controller = "Account", action = "SignIn", returnUrl = filterContext.HttpContext.Request.Url.ToString() }));
        }
    }
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public Guid EmployeeId { get; set; }
        public Role RoleId { get; set; }
        public string UserName { get; set; }
        public string UserScreens { get; set; }

    }

}