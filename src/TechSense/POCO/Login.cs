using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense.POCO
{
    public class Login
    {
        [Required(AllowEmptyStrings =false, ErrorMessage ="Please enter username.")]
        public string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter password.")]
        public string Password { get; set; }
    }
}
