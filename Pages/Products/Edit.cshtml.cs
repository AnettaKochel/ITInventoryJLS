using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;

namespace ITInventoryJLS.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Waps Waps { get; set; } = new Waps();

        public List<SelectListItem> LocationOptions { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var wapsFromDb = await _context.Waps.FindAsync(id);
            if (wapsFromDb == null) return NotFound();

            Waps = wapsFromDb;

            LocationOptions = await _context.Locations
                .AsNoTracking()
                .OrderBy(l => l.LocationName)
                .Select(l => new SelectListItem { Value = l.LocationName, Text = l.LocationName })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) {
                LocationOptions = await _context.Locations
                    .AsNoTracking()
                    .OrderBy(l => l.LocationName)
                    .Select(l => new SelectListItem { Value = l.LocationName, Text = l.LocationName })
                    .ToListAsync();
                return Page();
            }

            // Validate selected DeploymentLocation is present in Locations
            var exists = await _context.Locations.AnyAsync(l => l.LocationName == Waps.DeploymentLocation);
            if (!exists)
            {
                ModelState.AddModelError("Waps.DeploymentLocation", "Invalid location selected.");
                LocationOptions = await _context.Locations
                    .AsNoTracking()
                    .OrderBy(l => l.LocationName)
                    .Select(l => new SelectListItem { Value = l.LocationName, Text = l.LocationName })
                    .ToListAsync();
                return Page();
            }

            _context.Attach(Waps).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Waps.Any(e => e.Id == Waps.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Products/Index");
        }
    }
}
