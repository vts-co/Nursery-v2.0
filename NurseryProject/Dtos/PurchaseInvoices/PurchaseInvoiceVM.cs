using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.PurchaseInvoices
{
    public class PurchaseInvoiceVM
    {
        public Guid? SupplierId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double Total { get; set; }
        public string InvoiceNumber { get; set; }

        public List<PurchaseInvoiceItemVM> Items { get; set; }
    }

    public class PurchaseInvoiceItemVM
    {
        public Guid ItemId { get; set; }
        public double Quantity { get; set; }
        public double PurchasePrice { get; set; }
        public double Amount { get; set; }
    }

}