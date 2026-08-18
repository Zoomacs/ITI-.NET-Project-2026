using ITI_Project.Data;
using ITI_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITI_Project.Pages.Departments
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public Department Department { get; set; } = new Department();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Department = await _context.Departments.FindAsync(id)!;

            if (Department == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}