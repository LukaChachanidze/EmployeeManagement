using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [NotMapped] // so it wont be mapped to database
        public string EncryptedId { get; set; } 

        [Required]
        [MaxLength(50, ErrorMessage = "Name can't exceed more than 50 charecters")]
        public string Name { get; set; }
        
        [Display(Name = "Office Mail")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
        ErrorMessage = "Invalid email format")]
        [Required]
        public string Email { get; set; }
        [Required]
        
        public Dept? Department { get; set; }
        public string PhotoPath { get; set; }
        


    }
}
