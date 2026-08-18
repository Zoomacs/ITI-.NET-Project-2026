using System.Linq;
using ITI_Project.Data;
using ITI_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ITI_Project.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public IList<Department> Departments { get; set; } = new List<Department>();

        public async Task OnGetAsync()
        {
            var query = _context.Departments.AsQueryable();
            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(d => d.Name.Contains(SearchString));
            }

            Departments = await query.ToListAsync();
        }
    }
}