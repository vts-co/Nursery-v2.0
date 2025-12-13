using NurseryProject.Authorization;
using NurseryProject.Models;
using NurseryProject.Services.SupplierGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "4")]

    public class SupplierGroupController : Controller
    {
        // GET: SupplierGroup
        SupplierGroupServices GroupServices = new SupplierGroupServices();
        // GET: SupplierGroups
        public ActionResult Index()
        {
            var model = GroupServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {


            return View("Upsert", new SupplierGroup());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(SupplierGroup SupplierGroup)
        {
            SupplierGroup.Id = Guid.NewGuid();
            var result = GroupServices.Create(SupplierGroup, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                SupplierGroup.Id = Guid.Empty;


                TempData["warning"] = result.Message;
                return View("Upsert", SupplierGroup);
            }
        }
        public ActionResult Edit(Guid Id)
        {


            var SupplierGroup = GroupServices.Get(Id);
            return View("Upsert", SupplierGroup);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(SupplierGroup SupplierGroup)
        {

            var result = GroupServices.Edit(SupplierGroup, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {


                TempData["warning"] = result.Message;
                return View("Upsert", SupplierGroup);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = GroupServices.Delete(Id, (Guid)TempData["UserId"]);
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