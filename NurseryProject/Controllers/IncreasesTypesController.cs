using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.IncreasesTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "47")]

    public class IncreasesTypesController : Controller
    {
        IncreasesTypesServices increasesTypesServices = new IncreasesTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = increasesTypesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new IncreasesType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(IncreasesType increasesType)
        {
            increasesType.Id = Guid.NewGuid();
            var result = increasesTypesServices.Create(increasesType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                increasesType.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", increasesType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var increasesType = increasesTypesServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", increasesType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(IncreasesType increasesType)
        {

            var result = increasesTypesServices.Edit(increasesType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", increasesType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = increasesTypesServices.Delete(Id, (Guid)TempData["UserId"]);
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