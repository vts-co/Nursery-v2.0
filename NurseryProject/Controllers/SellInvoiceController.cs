using NurseryProject.Authorization;
using NurseryProject.Dtos.PurchaseInvoices;
using NurseryProject.Models;
using NurseryProject.Services.SellInvoices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "4")]

    public class SellInvoiceController : Controller
    {
        SellInvoiceServices sellInvoiceServices = new SellInvoiceServices();
        // GET: SellInvoice
        public ActionResult Index()
        {


            return View(sellInvoiceServices.GetAll());
        }



        public ActionResult Create()
        {
            using (var db = new almohandes_DbEntities())
            {
                ViewBag.StudentId = new SelectList(
                    db.Students.Where(x => !x.IsDeleted).ToList(),
                    "Id",
                    "Name"
                );

                ViewBag.StoreId = new SelectList(
                    db.Stores.Where(x => !x.IsDeleted).ToList(),
                    "Id",
                    "Name"
                );

                ViewBag.itemId = new SelectList(db.Items.Where(x => !x.IsDeleted).ToList(), "Id", "Name");

            }

            return View("Upsert", new PurchaseInvoicePostVM
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
                var items = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<List<PurchaseInvoiceItemVM>>(model.ItemsJson);
                ViewBag.ItemsJson = model.ItemsJson;

                if (model.StoreId == Guid.Empty || model.StoreId == null)
                {
                    TempData["warning"] = "من فضلك اختر المخزن";
                    LoadLists(); // إعادة تحميل الدروب داون
                    return View("Upsert", model);
                }
                if (model.StudentId == Guid.Empty|| model.StudentId==null)
                {
                    TempData["warning"] = "من فضلك اختر المخزن";
                    LoadLists(); // إعادة تحميل الدروب داون
                    return View("Upsert", model);
                }
             

                if (items == null || !items.Any())
                {
                    TempData["warning"] = "الأصناف غير صحيحة";
                    LoadLists();
                    return View("Upsert", model);
                }

                // 2️⃣ معالجة الـ Supplier لو فاضي
                Guid? StudentId = null;
                if (model.StudentId != Guid.Empty)
                    StudentId = model.StudentId;

                using (var db = new almohandes_DbEntities())
                using (var tx = db.Database.BeginTransaction())
                {
                    if (string.IsNullOrEmpty(model.InvoiceNumber))
                    {
                        model.InvoiceNumber = (db.SalesInvoices.Count() + 1).ToString();
                    }
                    try
                    {
                        // 3️⃣ إنشاء الفاتورة
                        var invoice = new SalesInvoice
                        {
                            Id = Guid.NewGuid(),
                            StudentId = (Guid)StudentId,
                            StoreId = model.StoreId,
                            InvoiceNumber = model.InvoiceNumber,
                            InvoiceDate = model.InvoiceDate,
                            Discount = model.Discount,
                            CreatedOn = DateTime.Now,
                            CreatedBy = (Guid)TempData["UserId"],
                            IsDeleted = false
                        };

                        db.SalesInvoices.Add(invoice);
                        db.SaveChanges();

                        // 4️⃣ إضافة تفاصيل الفاتورة
                        double total = 0;

                        foreach (var item in items)
                        {
                            var amount = (item.Quantity * item.PurchasePrice);
                            total += amount;

                            db.SalesInvoiceDetails.Add(new SalesInvoiceDetail
                            {
                                Id = Guid.NewGuid(),
                                SalesInvoiceId = invoice.Id,
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

                        TempData["success"] = "تم حفظ فاتورة  بنجاح ✔";
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
                ViewBag.StudentId = new SelectList(db.Students.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
                ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
                ViewBag.itemId = new SelectList(db.Items.Where(x => !x.IsDeleted).ToList(), "Id", "Name");
            }
        }
        public ActionResult Edit(Guid id)
        {
            using (var db = new almohandes_DbEntities())
            {
                var invoice = db.SalesInvoices
                    .Include("SalesInvoiceDetails")
                    .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
                var model = db.SalesInvoices
                  .Include("SalesInvoiceDetails").Where(x => x.Id == id && !x.IsDeleted).Select(x => new PurchaseInvoicePostVM
                  {

                      Id=x.Id,
                      StoreId= (Guid)x.StoreId,
                      StudentId=x.StudentId,
                      Discount=x.Discount,
                      InvoiceDate=x.InvoiceDate,
                      InvoiceNumber=x.InvoiceNumber,
                      
                  })
                  .FirstOrDefault();
                if (invoice == null)
                {
                    TempData["warning"] = "الفاتورة غير موجودة";
                    return RedirectToAction("Index");
                }

                // تحميل القوائم
                //LoadLists();
                ViewBag.StudentId = new SelectList(db.Students.Where(x => !x.IsDeleted).ToList(), "Id", "Name", invoice.StudentId);
                ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted).ToList(), "Id", "Name", invoice.StoreId);
                ViewBag.itemId = new SelectList(db.Items.Where(x => !x.IsDeleted).ToList(), "Id", "Name");


                var itemsVm = invoice.SalesInvoiceDetails.Where(x => !x.IsDeleted).Select(d => new
                {
                    ItemId = d.ItemId,
                    Quantity = d.Quantity,
                    PurchasePrice = d.Price,
                    Amount = d.Amount,
                    ItemName = d.Item.Name   // ⭐ أهو ده اللي نحتاجه
                }).ToList();

                ViewBag.ItemsJson = Newtonsoft.Json.JsonConvert.SerializeObject(itemsVm);


                return View("Upsert", model);
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
                    var invoice = db.SalesInvoices
                    .Include("SalesInvoiceDetails")
                    .FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);


                    ViewBag.ItemsJson = model.ItemsJson;
                    if (string.IsNullOrEmpty(model.ItemsJson))
                    {
                        TempData["warning"] = "من فضلك أضف صنف واحد على الأقل";
                        LoadLists();


                        return View("Upsert", model);

                    }

                    var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PurchaseInvoiceItemVM>>(model.ItemsJson);

                    if (items == null || !items.Any())
                    {
                        TempData["warning"] = "الأصناف غير صحيحة";
                        LoadLists();
                        return View("Upsert", model);
                    }

                    if (model.StoreId == null || model.StoreId == Guid.Empty)
                    {
                        TempData["warning"] = "تأكد من اختيار المخزن";
                        LoadLists();
                        return View("Upsert", model);
                    }


                    if (invoice == null)
                    {
                        TempData["warning"] = "الفاتورة غير موجودة";
                        return RedirectToAction("Index");
                    }

                    invoice.StudentId = (Guid)model.StudentId;
                    invoice.StoreId = model.StoreId;
                    invoice.InvoiceNumber = model.InvoiceNumber;
                    invoice.InvoiceDate = model.InvoiceDate;
                    invoice.Discount = model.Discount;
                    invoice.ModifiedOn = DateTime.Now;
                    invoice.ModifiedBy = (Guid)TempData["UserId"];

                    // حذف التفاصيل القديمة
                    var olditems = db.SalesInvoiceDetails.Where(x => x.SalesInvoiceId == model.Id).ToList();
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

                        db.SalesInvoiceDetails.Add(new SalesInvoiceDetail
                        {
                            Id = Guid.NewGuid(),
                            SalesInvoiceId = invoice.Id,
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
                    var invoice = db.SalesInvoices
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
                    var details = db.SalesInvoiceDetails
                        .Where(x => x.SalesInvoiceId == id && !x.IsDeleted)
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
        public JsonResult GetItemPrice(Guid id,Guid StoreId)
        {
            using (var db = new almohandes_DbEntities())
            {
                var PurchaseInvoice = db.PurchaseInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == id&&x.PurchaseInvoice.StoreId== StoreId).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();
                var SalesInvoice = db.SalesInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == id && x.SalesInvoice.StoreId == StoreId).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();

                var quantity = PurchaseInvoice - SalesInvoice;

                var item = db.Items
                    .Where(x => x.Id == id)
                    .Select(x => new
                    {
                        SellPrice = x.SellPrice,
                        PurchasePrice = x.PurchasePrice,
                        quantity= quantity
                    })
                    .FirstOrDefault();


                return Json(item, JsonRequestBehavior.AllowGet);
            }
        }

    }
}