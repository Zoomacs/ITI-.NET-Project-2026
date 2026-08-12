using ITI_Project.Data;
using ITI_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITI_Project.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.JobTitles = await _context.JobTitles.ToListAsync();

            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.JobTitles = await _context.JobTitles.ToListAsync();

            return View(employee);
        }


        // GET: Employee/Edit/5
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

// POST: Employee/Edit/5
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
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    ViewBag.Departments = await _context.Departments.ToListAsync();
    ViewBag.JobTitles = await _context.JobTitles.ToListAsync();

    return View(employee);
}

// GET: Employee/Delete/5
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

// POST: Employee/Delete/5
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

    return RedirectToAction("Index", "Home");
}
    }
}