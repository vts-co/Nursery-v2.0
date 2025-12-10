using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Items
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public double SellPrice { get; set; }
        public double PurchasePrice { get; set; }
    }
}