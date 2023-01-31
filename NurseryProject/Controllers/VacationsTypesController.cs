using NurseryProject.Authorization;
using NurseryProject.Services.VacationsTypes;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class VacationsTypesController : Controller
    {
        VacationsTypesServices vacationsTypesServices = new VacationsTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = vacationsTypesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new VacationsType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(VacationsType vacationsType)
        {
            vacationsType.Id = Guid.NewGuid();
            var result = vacationsTypesServices.Create(vacationsType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                vacationsType.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", vacationsType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var vacationsType = vacationsTypesServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", vacationsType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(VacationsType vacationsType)
        {

            var result = vacationsTypesServices.Edit(vacationsType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", vacationsType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = vacationsTypesServices.Delete(Id, (Guid)TempData["UserId"]);
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