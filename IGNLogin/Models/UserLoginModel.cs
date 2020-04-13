using System.ComponentModel.DataAnnotations;

namespace IGNLogin.Models
{
    public class UserLoginModel
    {
        
        public string Email { get; set; }

        public string Password { get; set; }

        public string AdminCode { get; set; }
    }
}
