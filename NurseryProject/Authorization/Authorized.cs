using NurseryProject.Enums;
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
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            if (controller != null)
            {
                VTSAuth auth = new VTSAuth() { CookieValues = new UserInfo { } };
                if (!auth.LoadDataFromCookies())
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {  controller = "Account", action = "SignIn", returnUrl = filterContext.HttpContext.Request.Url.ToString() }));
                    return;
                }
                else if ((auth.CookieValues.RoleId & Role) == 0)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {  controller = "Account", action = "SignIn", returnUrl = filterContext.HttpContext.Request.Url.ToString() }));

                }
                filterContext.Controller.TempData["UserInfo"] = auth.CookieValues;
                filterContext.Controller.TempData["UserId"] = auth.CookieValues.UserId;

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
        public Role RoleId { get; set; }
        public string UserName { get; set; }
       
    }
   
}