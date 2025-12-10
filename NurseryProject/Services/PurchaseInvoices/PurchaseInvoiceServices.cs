using NurseryProject.Dtos.PurchaseInvoices;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.PurchaseInvoices
{
    public class PurchaseInvoiceServices
    {
        public List<PurchaseInvoice> GetAll()
        {
            using (var db = new almohandes_DbEntities())
            {
                return db.PurchaseInvoices
                         .Where(x => !x.IsDeleted)
                         .OrderByDescending(x => x.InvoiceDate)
                         .ToList();
            }
        }

        public PurchaseInvoice Get(Guid id)
        {
            using (var db = new almohandes_DbEntities())
            {
                return db.PurchaseInvoices
                         .Include("PurchaseInvoiceItems")
                         .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            }
        }

        public ResultDto<PurchaseInvoice> Add(PurchaseInvoiceVM vm, Guid userId)
        {
            var result = new ResultDto<PurchaseInvoice>();

            using (var db = new almohandes_DbEntities())
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    var invoice = new PurchaseInvoice
                    {
                        Id = Guid.NewGuid(),
                        SupplierId = vm.SupplierId,
                        InvoiceNumber = DateTime.Now.Ticks.ToString(),
                        InvoiceDate = vm.InvoiceDate,
                        CreatedOn = DateTime.Now,

                        CreatedBy = userId
                    };

                    db.PurchaseInvoices.Add(invoice);
                    db.SaveChanges();

                    double total = 0;

                    foreach (var item in vm.Items)
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
                            CreatedBy = userId
                        });
                    }

                    invoice.TotalAmount = total;
                    db.SaveChanges();
                    tx.Commit();
                    result.IsSuccess = true;
                    result.Message = "تم حفظ البيانات بنجاح";
                    return result;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public void Update(Guid id, PurchaseInvoiceVM vm, Guid userId)
        {
            using (var db = new almohandes_DbEntities())
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    var invoice = db.PurchaseInvoices
                                    .Include("PurchaseInvoiceItems")
                                    .First(x => x.Id == id && !x.IsDeleted);

                    invoice.SupplierId = vm.SupplierId;
                    invoice.InvoiceDate = vm.InvoiceDate;
                    invoice.ModifiedOn = DateTime.Now;
                    invoice.ModifiedBy = userId;

                    // حذف التفاصيل القديمة
                    var olditem = db.PurchaseInvoiceDetails
                                .Where(x => x.PurchaseInvoiceId == id)
                                .ToList();
                    foreach (var item in olditem)
                    {
                        item.IsDeleted = true;
                    }

                    double total = 0;

                    foreach (var item in vm.Items)
                    {
                        var amount = item.Quantity * item.PurchasePrice;
                        total += amount;

                        db.PurchaseInvoiceDetails.Add(new PurchaseInvoiceDetail
                        {
                            Id = Guid.NewGuid(),
                            PurchaseInvoiceId = id,
                            ItemId = item.ItemId,
                            Quantity = item.Quantity,
                            Price = item.PurchasePrice,
                            Amount = amount,
                            CreatedOn = DateTime.Now,
                            CreatedBy = userId
                        });
                    }

                    invoice.TotalAmount = total;
                    db.SaveChanges();
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public void Delete(Guid id, Guid userId)
        {
            using (var db = new almohandes_DbEntities())
            {
                var invoice = db.PurchaseInvoices.First(x => x.Id == id);

                invoice.IsDeleted = true;
                invoice.DeletedOn = DateTime.Now;
                invoice.DeletedBy = userId;

                db.SaveChanges();
            }
        }


    }
}