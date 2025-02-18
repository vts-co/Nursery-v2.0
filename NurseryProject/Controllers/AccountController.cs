using NurseryProject.Authorization;
using NurseryProject.Dtos.Account;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services;
using NurseryProject.Services.Settings;
using NurseryProject.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
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
            var pass = Security.Decrypt("Q+0BSEUa0qVOJFYz3wEI6w==");

            using (var dbContext = new almohandes_DbEntities())
            {
                var Settingdata = dbContext.Settings.Where(x => !x.IsDeleted).FirstOrDefault();
                if (Settingdata != null)
                {
                    if (Settingdata.ActivationKey == null)
                        ViewBag.isactive = false;
                    else
                        ViewBag.isactive = true;
                }
                else
                    ViewBag.isactive = false;
            }
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





        #region Active/Un Offline Mode
        // تفعيل الموقع افلاين
        [HttpPost]
        public ActionResult Activation(string activePass)
        {
            try
            {
                using (var dbContext = new almohandes_DbEntities())
                {
                    if (string.IsNullOrEmpty(activePass))
                    {
                        TempData["MsgErrorActive"] = "رقم التفعيل خاطئ";
                        return RedirectToAction("SignIn");
                    }
                    var sas = "@vts-co@" + DateTime.Now.Date.ToString("ddMMyyyy");
                    string hashedValue = MachineDataService.CreateMD5(sas);
                    var OnlinehashedValue = MachineDataService.CreateMD5("@vts-co@online");

                    // string hashedValue = "F854C93C2F3F8D34257DD9B07024FB4B";

                    var hashedString = MachineDataService.CreateMD5(activePass);
                    if (hashedString == hashedValue)
                    {
                        string pcSerial = MachineDataService.getSerialID();
                        string serialEncrypted = Security.Encrypt(pcSerial);
                        var model = dbContext.Settings.Where(x => !x.IsDeleted ).FirstOrDefault();
                        model.ActivationKey = serialEncrypted;
                        model.IsOffline = true;
                        dbContext.Entry(model).State = EntityState.Modified;
                             dbContext.SaveChanges() ;
                      
                            TempData["MsgSuccessActive"] = "تم التفعيل بنجاح";
                            return RedirectToAction("SignIn");
                    
                    }
                    else if (OnlinehashedValue == hashedString)
                    {
                        var model1 = dbContext.Settings.Where(x => !x.IsDeleted ).FirstOrDefault();
                        model1.ActivationKey = MachineDataService.CreateMD5("@vts-co@online");
                        model1.IsOffline = false;
                       dbContext.Entry(model1).State = EntityState.Modified;
                        dbContext.Entry(model1).State = EntityState.Modified;
                        dbContext.SaveChanges();

                        TempData["success"] = "تم التفعيل بنجاح";
                            return RedirectToAction("SignIn");
                       

                    }
                    else
                    {
                        TempData["warning"] = "رقم التفعيل خاطئ";
                        return RedirectToAction("SignIn");
                    }
                }
            }
            catch (Exception ex)
            {
                {
                    TempData["warning"] = ex.Message;
                    return RedirectToAction("SignIn");
                }
            }

        }

 

        #endregion







    }
}