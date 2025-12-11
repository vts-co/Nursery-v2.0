using NurseryProject.Authorization;
using NurseryProject.Dtos.PurchaseInvoices;
using NurseryProject.Models;
using NurseryProject.Services.PurchaseInvoices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            try
            {
                // 1️⃣ التحقق من الأصناف
                if (string.IsNullOrEmpty(model.ItemsJson))
                {
                    TempData["warning"] = "من فضلك أضف صنف واحد على الأقل";
                    LoadLists(); // إعادة تحميل الدروب داون
                    return View("Upsert", model);
                }
                if (model.StoreId == Guid.Empty || model.StoreId == null)
                {
                    TempData["warning"] = "من فضلك اختر المخزن";
                    LoadLists(); // إعادة تحميل الدروب داون
                    return View("Upsert", model);
                }
                var items = Newtonsoft.Json.JsonConvert
                              .DeserializeObject<List<PurchaseInvoiceItemVM>>(model.ItemsJson);

                if (items == null || !items.Any())
                {
                    TempData["warning"] = "الأصناف غير صحيحة";
                    LoadLists();
                    return View("Upsert", model);
                }

                // 2️⃣ معالجة الـ Supplier لو فاضي
                Guid? supplierId = null;
                if (model.SupplierId != Guid.Empty)
                    supplierId = model.SupplierId;

                using (var db = new almohandes_DbEntities())
                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 3️⃣ إنشاء الفاتورة
                        var invoice = new PurchaseInvoice
                        {
                            Id = Guid.NewGuid(),
                            SupplierId = supplierId,
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

                        // 4️⃣ إضافة تفاصيل الفاتورة
                        double total = 0;

                        foreach (var item in items)
                        {
                            var amount = (item.Quantity * item.PurchasePrice);
                            total += amount;

                            db.PurchaseInvoiceDetails.Add(new PurchaseInvoiceDetail
                            {
                                Id = Guid.NewGuid(),
                                PurchaseInvoiceId = invoice.Id,
                                ItemId = item.ItemId,
                                Quantity = item.Quantity,
                                Price = item.PurchasePrice,
                                Amount = amount,
                                CreatedOn = DateTime.Now,
                                CreatedBy = (Guid)TempData["UserId"],
                                IsDeleted = false
                            });
                        }

                        // 5️⃣ حساب الإجماليات
                        invoice.TotalAmount = total;
                        invoice.Safy = total - model.Discount;

                        db.SaveChanges();
                        tx.Commit();

                        TempData["success"] = "تم حفظ فاتورة المشتريات بنجاح ✔";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        TempData["warning"] = ex.Message;

                        LoadLists();
                        return View("Upsert", model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["warning"] = ex.Message;
                LoadLists();
                return View("Upsert", model);
            }
        }
        private void LoadLists()
        {
            using (var db = new almohandes_DbEntities())
            {
                ViewBag.SupplierId = new SelectList(db.Suppliers.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
                ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
                ViewBag.Items = db.Items.Where(x => !x.IsDeleted).ToList();
            }
        }
        public ActionResult Edit(Guid id)
        {
            using (var db = new almohandes_DbEntities())
            {
                var invoice = db.PurchaseInvoices
                    .Include("PurchaseInvoiceDetails")
                    .FirstOrDefault(x => x.Id == id && !x.IsDeleted);

                if (invoice == null)
                {
                    TempData["warning"] = "الفاتورة غير موجودة";
                    return RedirectToAction("Index");
                }

                // تحميل القوائم
                //LoadLists();
                ViewBag.SupplierId = new SelectList(db.Suppliers.Where(x => !x.IsDeleted).ToList(), "Id", "Name", invoice.SupplierId);
                ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted).ToList(), "Id", "Name", invoice.StoreId);
                ViewBag.Items = db.Items.Where(x => !x.IsDeleted).ToList();


                var itemsVm = invoice.PurchaseInvoiceDetails.Where(x=>!x.IsDeleted).Select(d => new
                {
                    ItemId = d.ItemId,
                    Quantity = d.Quantity,
                    PurchasePrice = d.Price,
                    Amount = d.Amount,
                    ItemName = d.Item.Name   // ⭐ أهو ده اللي نحتاجه
                }).ToList();

                ViewBag.ItemsJson = Newtonsoft.Json.JsonConvert.SerializeObject(itemsVm);


                return View("Upsert", invoice);
            }
        }
        [HttpPost]
        public ActionResult Edit(PurchaseInvoicePostVM model)
        {
            try
            {
                using (var db = new almohandes_DbEntities())
                using (var tx = db.Database.BeginTransaction())
                {
                    var invoice = db.PurchaseInvoices
                    .Include("PurchaseInvoiceDetails")
                    .FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);
                

                    ViewBag.ItemsJson = model.ItemsJson;
                    if (string.IsNullOrEmpty(model.ItemsJson))
                    {
                        TempData["warning"] = "من فضلك أضف صنف واحد على الأقل";
                        LoadLists();


                        return View("Upsert", invoice);

                    }

                    var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PurchaseInvoiceItemVM>>(model.ItemsJson);

                    if (items == null || !items.Any())
                    {
                        TempData["warning"] = "الأصناف غير صحيحة";
                        LoadLists();
                        return View("Upsert", invoice);
                    }

                    if (model.StoreId == null || model.StoreId==Guid.Empty)
                    {
                        TempData["warning"] = "تأكد من اختيار المخزن";
                        LoadLists();
                        return View("Upsert", invoice);
                    }


                    if (invoice == null)
                    {
                        TempData["warning"] = "الفاتورة غير موجودة";
                        return RedirectToAction("Index");
                    }

                    invoice.SupplierId = model.SupplierId == Guid.Empty ? (Guid?)null : model.SupplierId;
                    invoice.StoreId = model.StoreId;
                    invoice.InvoiceNumber = model.InvoiceNumber;
                    invoice.InvoiceDate = model.InvoiceDate;
                    invoice.Discount = model.Discount;
                    invoice.ModifiedOn = DateTime.Now;
                    invoice.ModifiedBy = (Guid)TempData["UserId"];

                    // حذف التفاصيل القديمة
                    var olditems = db.PurchaseInvoiceDetails.Where(x => x.PurchaseInvoiceId == model.Id).ToList();
                    foreach (var item in olditems)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                    double total = 0;

                    foreach (var item in items)
                    {
                        var amount = item.Quantity * item.PurchasePrice;
                        total += amount;

                        db.PurchaseInvoiceDetails.Add(new PurchaseInvoiceDetail
                        {
                            Id = Guid.NewGuid(),
                            PurchaseInvoiceId = invoice.Id,
                            ItemId = item.ItemId,
                            Quantity = item.Quantity,
                            Price = item.PurchasePrice,
                            Amount = amount,
                            CreatedOn = DateTime.Now,
                            CreatedBy = (Guid)TempData["UserId"],
                            IsDeleted = false
                        });
                    }

                    invoice.TotalAmount = total;
                    invoice.Safy = total - invoice.Discount;

                    db.SaveChanges();
                    tx.Commit();

                    TempData["success"] = "تم تعديل الفاتورة بنجاح ✔";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["warning"] = ex.Message;
                LoadLists();
                return View("Upsert", model);
            }
        }
        public ActionResult Delete(Guid id)
        {
            try
            {
                using (var db = new almohandes_DbEntities())
                {
                    var invoice = db.PurchaseInvoices
                        .FirstOrDefault(x => x.Id == id && !x.IsDeleted);

                    if (invoice == null)
                    {
                        TempData["warning"] = "الفاتورة غير موجودة أو تم حذفها سابقًا";
                        return RedirectToAction("Index");
                    }

                    // Soft Delete
                    invoice.IsDeleted = true;
                    invoice.DeletedOn = DateTime.Now;
                    invoice.DeletedBy = (Guid)TempData["UserId"];

                    // حذف تفاصيل الفاتورة أيضًا (Soft delete)
                    var details = db.PurchaseInvoiceDetails
                        .Where(x => x.PurchaseInvoiceId == id && !x.IsDeleted)
                        .ToList();

                    foreach (var d in details)
                    {
                        d.IsDeleted = true;
                        d.DeletedOn = DateTime.Now;
                        d.DeletedBy = (Guid)TempData["UserId"];
                    }

                    db.SaveChanges();

                    TempData["success"] = "تم حذف الفاتورة بنجاح ✔";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["warning"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public JsonResult GetItemPrice(Guid id)
        {
            using (var db = new almohandes_DbEntities())
            {
                var item = db.Items
                    .Where(x => x.Id == id)
                    .Select(x => new
                    {
                        SellPrice = x.SellPrice,
                        PurchasePrice = x.PurchasePrice
                    })
                    .FirstOrDefault();

                return Json(item, JsonRequestBehavior.AllowGet);
            }
        }


    }
}