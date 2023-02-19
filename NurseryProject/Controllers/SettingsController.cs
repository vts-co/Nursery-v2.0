using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "2")]

    public class SettingsController : Controller
    {
        SettingsServices settingsServices = new SettingsServices();
        // GET: Settings
        public ActionResult Index()
        {
            var model = settingsServices.GetAll();
            return View(model);
        }
        public ActionResult Edit()
        {
            var setting = settingsServices.GetAll();
            return View("Upsert", setting);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Setting setting, HttpPostedFileBase Img)
        {
            if(setting.Id==Guid.Empty)
            {
                setting.Id = Guid.NewGuid();
            }
            if (Img != null)
            {
                setting.Logo = "/Uploads/Settings/";

                if (!Directory.Exists(Server.MapPath("~" + setting.Logo + setting.Id)))
                    Directory.CreateDirectory(Server.MapPath("~" + setting.Logo + setting.Id));
                setting.Logo = setting.Logo + setting.Id + "/" + setting.Id + ".jpg";
                Img.SaveAs(Server.MapPath("~" + setting.Logo));
            }
            var result = settingsServices.Edit(setting, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", setting);
            }
        }
    }
}