using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.PurchaseInvoices
{
    public class PurchaseInvoicePostVM
    {
        public Guid Id { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid StoreId { get; set; }

        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }

        public double Discount { get; set; }

        public string ItemsJson { get; set; }
    }

}