using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask2.Models
{
    public class SettingsViewModel
    {
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords are not match!")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public bool IsVisible { get; set; } = true;
    }
}
