using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class UserModel
    {
        public string name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string phonenumber { get; set; }
        public string password { get; set; }
        public string confirmpassword { get; set; }
        public string gender { get; set; }

    }
}