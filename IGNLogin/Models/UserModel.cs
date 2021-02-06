using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IGNLogin.Models
{
    public class UserModel
    {

        public string Login { get; set; }

        public string Email { get; set; }

        public string OfflineActivationCode { get; set; }

        public string Token { get; set; }
    }
}
