using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.DiscountsTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "49")]

    public class DiscountsTypesController : Controller
    {
        DiscountsTypesServices discountsTypesServices = new DiscountsTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = discountsTypesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new DiscountsType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(DiscountsType discountsType)
        {
            discountsType.Id = Guid.NewGuid();
            var result = discountsTypesServices.Create(discountsType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                discountsType.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", discountsType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var discountsType = discountsTypesServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", discountsType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(DiscountsType discountsType)
        {

            var result = discountsTypesServices.Edit(discountsType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", discountsType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = discountsTypesServices.Delete(Id, (Guid)TempData["UserId"]);
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