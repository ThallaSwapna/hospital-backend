using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WebApplication1.Models
{
    public class DoctorContext : DbContext
    {
        public DoctorContext() : base("DoctorDB") { }
        public DbSet<Doctor> Doctors { get; set; }
    }
}