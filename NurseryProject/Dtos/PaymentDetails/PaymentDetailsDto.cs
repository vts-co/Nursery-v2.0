using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Dtos.PaymentDetails
{
    public class PaymentDetailsDto
    {
        public Guid Id { get; set; }
        public Guid? StudyPlaceId { get; set; }
        public string StudyPlaceName { get; set; }

    }
}