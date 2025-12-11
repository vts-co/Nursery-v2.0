using NurseryProject.Dtos.PurchaseInvoices;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.SellInvoices
{
    public class SellInvoiceServices
    {
        public List<PurchaseInvoiceVM> GetAll()
        {
            using (var db = new almohandes_DbEntities())
            {
                return db.SalesInvoices
                         .Where(x => !x.IsDeleted)
                         .OrderByDescending(x => x.InvoiceDate).Select(x=>new PurchaseInvoiceVM { 
                        Id=x.Id,
                        StudentName=x.Student.Name,
                        InvoiceDate=x.InvoiceDate,
                        InvoiceNumber=x.InvoiceNumber,
                             TotalAmount=x.Safy
                         })
                         .ToList();
            }
        }
    }
}