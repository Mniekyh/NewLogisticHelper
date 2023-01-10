using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace LogisticHelper.Models
{
    public class AccountController
    {
        public static int userId;

        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public string EmailToResetPass { get; set; }
        public string NewPassword { get; set; }
        public string? token { get; set; }
        public string mailConfirmed { get; set; }
        public string? WrittenToken { get; set; }

        
        public string FileName { get; set; }

        public string user_id { get; set; }
        public string miejscowosc { get; set; }
        public string ulica { get; set; }
        public string Policy { get; set; }


        //public string? txtCon { get; set; }
        //public string? txtCity { get; set; }

        //public string? con { get; set; }
        //public string? city { get; set; }
        //public string? com { get; set; }

    }

}
