using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Second_Step.Data;
using Second_Step.Models;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Second_Step.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _appDbContext;
        private readonly EmployeeRepos _employee;

        public HomeController(ILogger<HomeController> logger, EmployeeRepos employeeRepos, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _employee = employeeRepos;
        }

        public IActionResult Index()
        {
            List<Employee> employees = _employee.GetEmployees();
            return View(employees);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult UploadCsv()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Read CSV using CsvHelper with a custom ClassMap
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";", // Set the delimiter to semicolon
                    HasHeaderRecord = true // Indicates the first row is a header
                }))
                {
                    csv.Context.RegisterClassMap<EmployeeMap>(); // Register the custom ClassMap

                    var records = csv.GetRecords<Employee>().ToList(); // Map the records to Employee model

                    foreach (var employee in records)
                    {
                        _appDbContext.Add(employee);
                    }

                    await _appDbContext.SaveChangesAsync();
                }
            }

            return View();
        }


        public IActionResult Edit(int id)
        {
            
            var employee = _appDbContext.Employees.Find(id);

            if (employee == null)
            {
                return NotFound(); 
            }

            return View(employee); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound(); 
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _appDbContext.Update(employee); 
                    await _appDbContext.SaveChangesAsync(); 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_appDbContext.Employees.Any(e => e.Id == id))
                    {
                        return NotFound(); 
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public IActionResult Delete(int id)
        {
            var employee = _appDbContext.Employees.Find(id);
            if (employee == null)
            {
                return NotFound(); 
            }
            return View(employee); 
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _appDbContext.Employees.FindAsync(id);
            if (employee != null)
            {
                _appDbContext.Employees.Remove(employee); 
                await _appDbContext.SaveChangesAsync(); 
            }
            return RedirectToAction(nameof(Index)); // Redirect to the employee list after deletion
        }
        public IActionResult Search(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var employees = _appDbContext.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.Forenames.Contains(searchString) || e.Surname.Contains(searchString));
            }

            return View("Index", employees.ToList()); // Return to the Index view
        }





        public class EmployeeMap : ClassMap<Employee>
        {
            public EmployeeMap()
            {
                Map(m => m.Id).Index(0);
                Map(m => m.Payroll_Number).Index(1);
                Map(m => m.Forenames).Index(2);
                Map(m => m.Surname).Index(3);
                Map(m => m.Date_of_Birth).Index(4).TypeConverterOption.Format("dd.MM.yyyy"); // Custom DateTime format
                Map(m => m.Telephone).Index(5);
                Map(m => m.Mobile).Index(6);
                Map(m => m.Address).Index(7);
                Map(m => m.Address_2).Index(8);
                Map(m => m.Postcode).Index(9);
                Map(m => m.EMail_Home).Index(10);
                Map(m => m.Start_Date).Index(11).TypeConverterOption.Format("dd.MM.yyyy"); // Custom DateTime format
            }
        }


    }
}
