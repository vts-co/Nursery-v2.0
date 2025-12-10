using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.Subscriptions
{
    public class SubscriptionsDto
    {
        public Guid Id { get; set; }
        public Guid StudyTypeId { get; set; }
        public string StudyTypeName { get; set; }
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public Guid SubscriptionTypeId { get; set; }
        public string SubscriptionTypeName { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public string Discount { get; set; }
        public string DiscountReason { get; set; }
        public string InstallmentsNumber { get; set; }
        public string InstallmentAmount { get; set; }
        public bool IsAnother { get; set; }

    }
}