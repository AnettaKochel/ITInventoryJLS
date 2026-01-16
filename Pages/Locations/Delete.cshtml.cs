using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;

namespace ITInventoryJLS.Pages.Locations
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Location Location { get; set; } = new Location();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var locationFromDb = await _context.Locations.FindAsync(id);
            if (locationFromDb == null) return NotFound();

            Location = locationFromDb;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Location == null || Location.LocationId == 0) return NotFound();

            var location = await _context.Locations.FindAsync(Location.LocationId);

            if (location != null)
            {
                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Locations/Index");
        }
    }
}
