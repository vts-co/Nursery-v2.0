using NurseryProject.Authorization;
using NurseryProject.Models;
using NurseryProject.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "39")]

    public class StoresController : Controller
    {
        StoreServices storeServices = new StoreServices();
        // GET: Stores
        public ActionResult Index()
        {
            var model = storeServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {


            return View("Upsert", new Store());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Store Store)
        {
            Store.Id = Guid.NewGuid();
            var result = storeServices.Create(Store, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                Store.Id = Guid.Empty;


                TempData["warning"] = result.Message;
                return View("Upsert", Store);
            }
        }
        public ActionResult Edit(Guid Id)
        {


            var Store = storeServices.Get(Id);
            return View("Upsert", Store);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Store Store)
        {

            var result = storeServices.Edit(Store, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {


                TempData["warning"] = result.Message;
                return View("Upsert", Store);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = storeServices.Delete(Id, (Guid)TempData["UserId"]);
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