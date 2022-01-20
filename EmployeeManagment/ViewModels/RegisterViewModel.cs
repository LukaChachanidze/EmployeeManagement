using EmployeeManagment.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")] 
        // Erorr message is coming from validation attribute class which we inherited in our class
        [ValidEmailDomain(allowedDomain: "gmail.com", ErrorMessage = "Use only gmail.com")]
        // we are telling this remote attribute which action method to use
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set;}

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",
            ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
        public string City { get; set; }


    }
}
