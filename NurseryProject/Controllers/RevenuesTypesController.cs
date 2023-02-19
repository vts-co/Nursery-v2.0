using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.RevenuesTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "42")]

    public class RevenuesTypesController : Controller
    {
        RevenuesTypesServices revenuesTypesServices = new RevenuesTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = revenuesTypesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            var RevenuesTypes = revenuesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty);
            ViewBag.RevenuesTypes = RevenuesTypes;

            return View("Upsert", new RevenuesType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(RevenuesType revenuesType)
        {
            revenuesType.Id = Guid.NewGuid();
            var result = revenuesTypesServices.Create(revenuesType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                revenuesType.Id = Guid.Empty;
                var RevenuesTypes = revenuesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty);
                ViewBag.RevenuesTypes = RevenuesTypes;
                TempData["warning"] = result.Message;
                return View("Upsert", revenuesType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var RevenuesTypes = revenuesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty);
            ViewBag.RevenuesTypes = RevenuesTypes;

            var revenuesType = revenuesTypesServices.Get(Id);
            return View("Upsert", revenuesType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(RevenuesType revenuesType)
        {
            var result = revenuesTypesServices.Edit(revenuesType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var RevenuesTypes = revenuesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty);
                ViewBag.RevenuesTypes = RevenuesTypes;

                TempData["warning"] = result.Message;
                return View("Upsert", revenuesType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = revenuesTypesServices.Delete(Id, (Guid)TempData["UserId"]);
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