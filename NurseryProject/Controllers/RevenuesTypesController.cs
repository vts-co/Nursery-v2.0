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
    [Authorized(Role = Role.SystemAdmin)]

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

                TempData["warning"] = result.Message;
                return View("Upsert", revenuesType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var revenuesType = revenuesTypesServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
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