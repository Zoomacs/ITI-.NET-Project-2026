using ITI_Project.Data;
using ITI_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITI_Project.Controllers
{
    public class JobTitleController : Controller
    {
        private readonly AppDbContext _context;

        public JobTitleController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.JobTitles.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(j => j.Title.Contains(searchString));
            }

            ViewBag.CurrentSearch = searchString;
            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var job = await _context.JobTitles.FindAsync(id);
            if (job == null) return NotFound();
            return View(job);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobTitle jobTitle)
        {
            if (ModelState.IsValid)
            {
                _context.JobTitles.Add(jobTitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobTitle);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var job = await _context.JobTitles.FindAsync(id);
            if (job == null) return NotFound();
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobTitle jobTitle)
        {
            if (id != jobTitle.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.JobTitles.Update(jobTitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobTitle);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var job = await _context.JobTitles.FindAsync(id);
            if (job == null) return NotFound();
            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.JobTitles.FindAsync(id);
            if (job != null)
            {
                _context.JobTitles.Remove(job);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}