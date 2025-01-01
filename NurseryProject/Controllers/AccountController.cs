using NurseryProject.Authorization;
using NurseryProject.Dtos.Account;
using NurseryProject.Enums;
using NurseryProject.Services;
using NurseryProject.Services.Settings;
using NurseryProject.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    public class AccountController : Controller
    {
        AccountServices accountService = new AccountServices();
        SettingsServices settingsServices = new SettingsServices();
        // GET: Account
        public ActionResult SignIn()
        {
            var model = settingsServices.GetAll();
            TempData["SettingLogo"] = model.Logo;
            TempData["SettingTitle"] = model.Title;
            var pass = Security.Encrypt("darsalah@123");

            return View(new SignInDto());
        }
        [HttpPost]
        public ActionResult SignIn(SignInDto userInfo)
        {
            var model = settingsServices.GetAll();
            TempData["SettingLogo"] = model.Logo;
            TempData["SettingTitle"] = model.Title;

            var result = accountService.Login(userInfo.UserName, userInfo.Password);
            if (result.IsSuccess)
            {
                VTSAuth auth = new VTSAuth();
                auth.SaveToCookies(result.Result);
                string returnUrl = Request.QueryString["returnUrl"];
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                TempData["success"] = result.Message;
                return RedirectToAction("Index", "Home");

            }
            else
            {
                TempData["warning"] = result.Message;
                userInfo.Password = "";
                return View(userInfo);
            }
        }
        public ActionResult SignOut()
        {
            VTSAuth auth = new VTSAuth();
            auth.LoadDataFromCookies();
            auth.ClearCookies();
            return RedirectToAction("Index", "Home");
        }
    }
}