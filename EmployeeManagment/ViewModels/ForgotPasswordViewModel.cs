using System.ComponentModel.DataAnnotations;

namespace EmployeeManagment.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
