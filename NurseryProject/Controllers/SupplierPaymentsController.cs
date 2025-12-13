using NurseryProject.Authorization;
using NurseryProject.Models;
using NurseryProject.Services.SupplierPayments;
using NurseryProject.Services.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "4")]

    public class SupplierPaymentsController : Controller
    {
        SupplierPayementServices PayementServices = new SupplierPayementServices();
        SupplierServices supplierServices = new SupplierServices();

        // GET: SupplierPayments
        public ActionResult Index()
        {
            var model = PayementServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            var Groups = supplierServices.GetAll();
            ViewBag.Groups = Groups;

            return View("Upsert", new SupplierPayment {PaymentDate=DateTime.Now });
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(SupplierPayment model)
        {
            model.Id = Guid.NewGuid();
            var result = PayementServices.Create(model, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                model.Id = Guid.Empty;
                var Groups = supplierServices.GetAll();
                ViewBag.Groups = Groups;

                TempData["warning"] = result.Message;
                return View("Upsert", model);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var Groups = supplierServices.GetAll();
            ViewBag.Groups = Groups;

            var Supplier = PayementServices .Get(Id);
            return View("Upsert", Supplier);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(SupplierPayment Supplier)
        {

            var result = PayementServices.Edit(Supplier, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var Groups = supplierServices.GetAll();
                ViewBag.Groups = Groups;

                TempData["warning"] = result.Message;
                return View("Upsert", Supplier);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = PayementServices.Delete(Id, (Guid)TempData["UserId"]);
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



        [HttpGet]
        public JsonResult GetSupplierBalance(Guid id)
        {
            using (var db = new almohandes_DbEntities())
            {
                var PurchaseInvoice = db.PurchaseInvoices.Where(x => !x.IsDeleted && x.SupplierId == id ).Select(x => x.Safy).DefaultIfEmpty(0).Sum();
                var SupplierPayments = db.SupplierPayments.Where(x => !x.IsDeleted && x.SupplierId == id ).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

                var quantity = PurchaseInvoice - SupplierPayments;

              


                return Json(new { quantity = quantity }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}