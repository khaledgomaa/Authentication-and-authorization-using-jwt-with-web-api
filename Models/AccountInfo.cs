using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jwt.Models
{
    public class AccountInfo
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }

    public class RegisterModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}