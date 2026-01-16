using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITInventoryJLS.Pages.Computers
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Computer> Computers { get; set; } = new List<Computer>();

        [BindProperty(SupportsGet = true)]
        public string? TypeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OSFilter { get; set; }

        public List<SelectListItem> TypeOptions { get; set; } = new();
        public List<SelectListItem> OSOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            // populate filter option lists (distinct values)
            TypeOptions = await _context.Computers
                .AsNoTracking()
                .Select(c => c.Type)
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .OrderBy(t => t)
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToListAsync();

            OSOptions = await _context.Computers
                .AsNoTracking()
                .Select(c => c.OSVersion)
                .Where(o => !string.IsNullOrEmpty(o))
                .Distinct()
                .OrderBy(o => o)
                .Select(o => new SelectListItem { Value = o, Text = o })
                .ToListAsync();

            var query = _context.Computers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(TypeFilter))
            {
                query = query.Where(c => c.Type == TypeFilter);
            }

            if (!string.IsNullOrWhiteSpace(OSFilter))
            {
                query = query.Where(c => c.OSVersion == OSFilter);
            }

            Computers = await query.OrderBy(c => c.DeviceName).ToListAsync();
        }
    }
}
