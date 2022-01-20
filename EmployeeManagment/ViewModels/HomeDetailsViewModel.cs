using EmployeeManagment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.ViewModels
{
    public class HomeDetailsViewModel
    {
        // We use this class when a model object doesn't contain all the data a view needs
        // For example Page Title and  model properties
        public Employee Employee { get; set; }
        public string PageTitle { get; set; }

    }
}
