using ITI_Project.Data;
using ITI_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITI_Project.Pages.Departments
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Department Department { get; set; } = new Department();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Department = await _context.Departments.FindAsync(id);

            if (Department == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var department = await _context.Departments.FindAsync(Department.Id);

            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}