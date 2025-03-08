using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class PaymentData
    {
        public int PatientId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }
}