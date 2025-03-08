﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Models
{
    public class Doctor
    {
        [Key]
        public int doctor_id { get; set; }
        public string name { get; set; }
        public string designation { get; set; }
        public string phonenumber { get; set; }
        public int age { get; set; }
        public string gender { get; set; }
    }
}