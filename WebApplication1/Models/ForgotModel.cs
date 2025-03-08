using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ForgotModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string confirmpassword { get; set; }
    }
}