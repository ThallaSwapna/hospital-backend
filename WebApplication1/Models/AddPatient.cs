using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Models
{
    public class AddPatient
    {
        [Key]
        public int patient_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phonenumber { get; set; }
        public string disease { get; set; }
        public int age { get; set; }
        public string gender { get; set; }

        public byte active { get; set; }
        public decimal dueamount { get; set; }
        public int doctorId { get; set; }

        public string doctor_name { get; set; }
    }

}