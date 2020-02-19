using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IGNLogin.Models
{
    public class UserModel
    {
        public string Email { get; set; }

        public long Id { get; set; }

        public bool IsActive { get; set; }

    }
}
