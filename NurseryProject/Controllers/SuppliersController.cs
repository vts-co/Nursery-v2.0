using NurseryProject.Authorization;
using NurseryProject.Models;
using NurseryProject.Services.SupplierGroups;
using NurseryProject.Services.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "4")]

    public class SuppliersController : Controller
    {
        // GET: Suppliers
        SupplierServices supplierServices = new SupplierServices();
        SupplierGroupServices GroupServices = new SupplierGroupServices();
        // GET: Suppliers
        // GET: Cities
        public ActionResult Index()
        {
            var model = supplierServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            var Groups = GroupServices.GetAll();
            ViewBag.Groups = Groups;

            return View("Upsert", new Supplier());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Supplier Supplier)
        {
            Supplier.Id = Guid.NewGuid();
            var result = supplierServices.Create(Supplier, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                Supplier.Id = Guid.Empty;
                var Groups = GroupServices.GetAll();
                ViewBag.Groups = Groups;

                TempData["warning"] = result.Message;
                return View("Upsert", Supplier);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var Groups = GroupServices.GetAll();
            ViewBag.Groups = Groups;

            var Supplier = supplierServices.Get(Id);
            return View("Upsert", Supplier);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Supplier Supplier)
        {

            var result = supplierServices.Edit(Supplier, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var Groups = GroupServices.GetAll();
                ViewBag.Groups = Groups;

                TempData["warning"] = result.Message;
                return View("Upsert", Supplier);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = supplierServices.Delete(Id, (Guid)TempData["UserId"]);
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