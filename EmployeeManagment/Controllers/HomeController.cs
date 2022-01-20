using EmployeeManagment.Models;
using EmployeeManagment.Security;
using EmployeeManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ILogger logger;
        private readonly IDataProtector protector;
        
        public HomeController(IEmployeeRepository employeeRepository,
            IWebHostEnvironment hostingEnvironment, ILogger<HomeController> 
            logger, IDataProtectionProvider  dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings)
        // Using Consturctor to inject service IEmployeeRepository (this is called constructor injection)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            protector = dataProtectionProvider
                .CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }
        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee()
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.Id.ToString());
                                return e;
                            });
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult Delete(int id)
        {
            //throw new Exception("Error");

            Employee employee = _employeeRepository.GetEmployee(id);

            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound");
            }

            var deletedEmployee = _employeeRepository.Delete(employee.Id);
            if (deletedEmployee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound");
            }
            return RedirectToAction("index");
        }

        [AllowAnonymous]
        public ViewResult Details(string id)
        {
            //throw new Exception("Error");
            



            int employeeId = Convert.ToInt32(protector.Unprotect(id));

            Employee employee = _employeeRepository.GetEmployee(employeeId);

            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", employeeId);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Create()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create(EmployeeCreateViewModel model)
        // This parameter employee matches the name input element, so does email and department 
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);


                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };
                _employeeRepository.Add(newEmployee);
                return RedirectToAction("index");
            }

            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(employeeEditViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
         // users can't edit anything unless they are logged in
        // Authorize attribute directs user to login page
        public IActionResult Edit(EmployeeEditViewModel model)
        // This parameter employee matches the name input element, so does email and department 
        {
            if (ModelState.IsValid) // No validation errors
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photo != null) // Checking if the current photo is null or not
                {
                    if (model.ExistingPhotoPath != null) // Removing the existing photo 
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                             "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath); 

                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }



                _employeeRepository.Update(employee);
                return RedirectToAction("index", new { id = employee.Id });
            }

            return View();

        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {

                // We get path to images folder in our wwwroot file and store it in a string
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + " " + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        

    }
            
        
    }
}