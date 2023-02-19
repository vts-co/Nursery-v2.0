using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Cities;
using NurseryProject.Services.Destricts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "5")]

    public class DestrictsController : Controller
    {
        CitiesServices citiesServices = new CitiesServices();
        DestrictsServices destrictsServices = new DestrictsServices();

        // GET: Destricts
        public ActionResult Index()
        {
            
            var model = destrictsServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var cityModel = citiesServices.GetAll();
            ViewBag.Cities = cityModel;

            return View("Upsert", new Destrict());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Destrict destrict)
        {
            destrict.Id = Guid.NewGuid();
            var result = destrictsServices.Create(destrict, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                destrict.Id = Guid.Empty;

                var cityModel = citiesServices.GetAll();
                ViewBag.Cities = cityModel;

                TempData["warning"] = result.Message;
                return View("Upsert", destrict);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var cityModel = citiesServices.GetAll();
            ViewBag.Cities = cityModel;

            var destrict = destrictsServices.Get(Id);
            return View("Upsert", destrict);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Destrict destrict)
        {

            var result = destrictsServices.Edit(destrict, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var cityModel = citiesServices.GetAll();
                ViewBag.Cities = cityModel;

                TempData["warning"] = result.Message;
                return View("Upsert", destrict);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = destrictsServices.Delete(Id, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return RedirectToAction("Index");
            }
        }
    }
}