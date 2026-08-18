using ITI_Project.Data;
using ITI_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace ITI_Project.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EmployeeController(AppDbContext context , IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string searchString, int? departmentId, int? jobTitleId)
        {
            var employeesQuery = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                employeesQuery = employeesQuery.Where(e =>
                    e.FullName.Contains(searchString) ||
                    e.Email.Contains(searchString));
            }

            if (departmentId.HasValue && departmentId > 0)
            {
                employeesQuery = employeesQuery.Where(e => e.DepartmentId == departmentId);
            }

            if (jobTitleId.HasValue && jobTitleId > 0)
            {
                employeesQuery = employeesQuery.Where(e => e.JobTitleId == jobTitleId);
            }

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.JobTitles = await _context.JobTitles.ToListAsync();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentDepartmentId = departmentId;
            ViewBag.CurrentJobTitleId = jobTitleId;

            var employees = await employeesQuery.ToListAsync();

            return View(employees);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

       public async Task<IActionResult> Create()
       {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.JobTitles = await _context.JobTitles.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee ,IFormFile? profileImage)
        {
            if (ModelState.IsValid)
            {
                if (profileImage != null && profileImage.Length > 0)
                  {
                      string uploadsFolder = Path.Combine
                      (
                       _environment.WebRootPath,
                          "images",
                          "employees"
                     );

        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);

        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await profileImage.CopyToAsync(fileStream);
        }

        employee.ProfileImagePath = "/images/employees/" + uniqueFileName;
    }
    else
    {
        employee.ProfileImagePath = "/images/employees/emp2.svg";
    }
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.JobTitles = await _context.JobTitles.ToListAsync();

            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.JobTitles = await _context.JobTitles.ToListAsync();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee, IFormFile? profileImage)        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
            if (profileImage != null && profileImage.Length > 0)
{
    string uploadsFolder = Path.Combine(
        _environment.WebRootPath,
        "images",
        "employees"
    );

    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);

    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

    using (var fileStream = new FileStream(filePath, FileMode.Create))
    {
        await profileImage.CopyToAsync(fileStream);
    }

    employee.ProfileImagePath = "/images/employees/" + uniqueFileName;
}
else
{
    var existingEmployee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

    bool isInitialsAvatar = existingEmployee != null
        && !string.IsNullOrEmpty(existingEmployee.ProfileImagePath)
        && existingEmployee.ProfileImagePath.StartsWith("/images/employees/")
        && existingEmployee.ProfileImagePath.EndsWith(".svg");

    if (existingEmployee != null
        && existingEmployee.FullName != employee.FullName
        && isInitialsAvatar)
    {
        string newImagePath = WriteInitialsSvg(GetInitials(employee.FullName));

        if (!string.IsNullOrEmpty(existingEmployee.ProfileImagePath)
            && existingEmployee.ProfileImagePath != "/images/employees/emp2.svg")
        {
            bool usedElsewhere = await _context.Employees
                .AnyAsync(e => e.Id != id && e.ProfileImagePath == existingEmployee.ProfileImagePath);

            if (!usedElsewhere)
            {
                string oldFullPath = Path.Combine(
                    _environment.WebRootPath,
                    existingEmployee.ProfileImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(oldFullPath))
                {
                    System.IO.File.Delete(oldFullPath);
                }
            }
        }

        employee.ProfileImagePath = newImagePath;
    }
    else
    {
        employee.ProfileImagePath = existingEmployee?.ProfileImagePath;
    }
}
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.JobTitles = await _context.JobTitles.ToListAsync();

            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .FirstOrDefaultAsync(e => e.Id == id);

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
            var employee = await _context.Employees.FindAsync(id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        private static string GetInitials(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                return $"{parts[0][0]}{parts[1][0]}".ToUpperInvariant();
            }

            if (parts.Length == 1)
            {
                return parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant();
            }

            return "?";
        }

        private string WriteInitialsSvg(string initials)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "employees");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = "emp" + Guid.NewGuid().ToString("N") + ".svg";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            string[] colors = { "#e74a3b", "#36b9cc", "#4e73df", "#1cc88a", "#f6c23e", "#858796" };
            string color = colors[Random.Shared.Next(colors.Length)];

            string svg =
                $"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 120 120' width='120' height='120'>\n" +
                $"  <rect width='120' height='120' fill='{color}'/>\n" +
                $"  <text x='50%' y='50%' dy='.35em' text-anchor='middle' font-family='Nunito, Segoe UI, Arial, sans-serif' font-size='46' font-weight='700' fill='#ffffff'>{initials}</text>\n" +
                $"</svg>";

            System.IO.File.WriteAllText(filePath, svg);

            return "/images/employees/" + uniqueFileName;
        }
    }
}
