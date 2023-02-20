using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.SubscriptionsTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "21")]

    public class SubscriptionsTypesController : Controller
    {
        SubscriptionsTypesServices subscriptionsTypesServices = new SubscriptionsTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = subscriptionsTypesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new SubscriptionsType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(SubscriptionsType subscriptionsType)
        {
            subscriptionsType.Id = Guid.NewGuid();
            var result = subscriptionsTypesServices.Create(subscriptionsType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                subscriptionsType.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", subscriptionsType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var subscriptionsType = subscriptionsTypesServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", subscriptionsType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(SubscriptionsType subscriptionsType)
        {

            var result = subscriptionsTypesServices.Edit(subscriptionsType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", subscriptionsType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = subscriptionsTypesServices.Delete(Id, (Guid)TempData["UserId"]);
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