using NurseryProject.Authorization;
using NurseryProject.Dtos.PurchaseInvoices;
using NurseryProject.Models;
using NurseryProject.Services.PurchaseInvoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "4")]

    public class PurchaseInvoiceController : Controller
    {
        PurchaseInvoiceServices purchaseInvoiceServices = new PurchaseInvoiceServices();
        // GET: PurchaseInvoice
        public ActionResult Index()
        {


            return View(purchaseInvoiceServices.GetAll());
        }



        public ActionResult Create()
        {
            using (var db = new almohandes_DbEntities())
            {
                ViewBag.SupplierId = new SelectList(
                    db.Suppliers.Where(x => !x.IsDeleted).ToList(),
                    "Id",
                    "Name"
                );

                ViewBag.StoreId = new SelectList(
                    db.Stores.Where(x => !x.IsDeleted).ToList(),
                    "Id",
                    "Name"
                );

                // خلي الاسم ViewBag.Items علشان مطابق للـ View
                ViewBag.Items = db.Items
                                  .Where(x => !x.IsDeleted)
                                  .ToList();
            }

            return View("Upsert", new PurchaseInvoice
            {
                InvoiceDate = DateTime.Now
            });
        }



        [HttpPost]
        public ActionResult Create(PurchaseInvoicePostVM model)
        {
            if (string.IsNullOrEmpty(model.ItemsJson))
            {
                ModelState.AddModelError("", "لم يتم إضافة أصناف");
                return View("Upsert", model);
            }

            var items = Newtonsoft.Json.JsonConvert
                          .DeserializeObject<List<PurchaseInvoiceItemVM>>(model.ItemsJson);

            if (items == null || !items.Any())
            {
                ModelState.AddModelError("", "الأصناف غير صحيحة");
                return View("Upsert", model);
            }

            using (var db = new almohandes_DbEntities())
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    if (model.SupplierId==Guid.Empty)
                    {
                        model.SupplierId = null;
                    }
                    // 1️⃣ إنشاء الفاتورة
                    var invoice = new PurchaseInvoice
                    {
                        Id = Guid.NewGuid(),
                        SupplierId = model.SupplierId,
                        StoreId = model.StoreId,
                        InvoiceNumber = model.InvoiceNumber,
                        InvoiceDate = model.InvoiceDate,
                        Discount = model.Discount,
                        CreatedOn = DateTime.Now,
                        CreatedBy = (Guid)TempData["UserId"],
                        IsDeleted = false
                    };

                    db.PurchaseInvoices.Add(invoice);
                    db.SaveChanges();

                    // 2️⃣ إضافة الأصناف
                    double total = 0;

                    foreach (var i in items)
                    {
                        var amount = i.Quantity * i.PurchasePrice;
                        total += amount;

                        db.PurchaseInvoiceDetails.Add(new PurchaseInvoiceDetail
                        {
                            Id = Guid.NewGuid(),
                            PurchaseInvoiceId = invoice.Id,
                            ItemId = i.ItemId,
                            Quantity = i.Quantity,
                            Price = i.PurchasePrice,
                            Amount = amount,
                            CreatedOn = DateTime.Now,
                            CreatedBy = (Guid)TempData["UserId"],
                            IsDeleted = false
                        });
                    }

                    // 3️⃣ الإجماليات
                    invoice.TotalAmount = total;
                    invoice.Safy = total - model.Discount;

                    db.SaveChanges();
                    tx.Commit();

                    return RedirectToAction("Details", new { id = invoice.Id });
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    ModelState.AddModelError("", ex.Message);
                    return View("Upsert", model);
                }
            }
        }



    }
}