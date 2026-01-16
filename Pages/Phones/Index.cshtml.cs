using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using Microsoft.AspNetCore.Mvc;
using ITInventoryJLS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITInventoryJLS.Pages.Phones
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Phone> Phones { get; set; } = new List<Phone>();

        [BindProperty(SupportsGet = true)]
        public string? ManufacturerFilter { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Phones.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(ManufacturerFilter))
            {
                query = query.Where(p => p.Manufacturer == ManufacturerFilter);
            }

            Phones = await query.OrderBy(p => p.Manufacturer).ThenBy(p => p.Model).ToListAsync();
        }
    }
}
