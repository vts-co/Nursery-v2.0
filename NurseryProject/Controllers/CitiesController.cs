using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Cities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]
    public class CitiesController : Controller
    {

        CitiesServices citiesServices = new CitiesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = citiesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new City());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(City city)
        {
            city.Id = Guid.NewGuid();
            var result = citiesServices.Create(city, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                city.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", city);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var city = citiesServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", city);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(City city)
        {
            
            var result = citiesServices.Edit(city, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", city);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = citiesServices.Delete(Id, (Guid)TempData["UserId"]);
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