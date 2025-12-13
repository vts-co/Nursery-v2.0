using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Supplier
{
    public class SupplierPaymentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}