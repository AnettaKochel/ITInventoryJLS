using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;
using System.Collections.Generic;

namespace ITInventoryJLS.Pages.Phones
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Phone Phone { get; set; } = new Phone();

        public List<SelectListItem> LocationOptions { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            LocationOptions = await _context.Locations
                .AsNoTracking()
                .OrderBy(l => l.LocationName)
                .Select(l => new SelectListItem { Value = l.LocationName, Text = l.LocationName })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LocationOptions = await _context.Locations
                    .AsNoTracking()
                    .OrderBy(l => l.LocationName)
                    .Select(l => new SelectListItem { Value = l.LocationName, Text = l.LocationName })
                    .ToListAsync();
                return Page();
            }

            var exists = await _context.Locations.AnyAsync(l => l.LocationName == Phone.DeploymentLocation);
            if (!exists)
            {
                ModelState.AddModelError("Phone.DeploymentLocation", "Invalid location selected.");
                LocationOptions = await _context.Locations
                    .AsNoTracking()
                    .OrderBy(l => l.LocationName)
                    .Select(l => new SelectListItem { Value = l.LocationName, Text = l.LocationName })
                    .ToListAsync();
                return Page();
            }

            _context.Phones.Add(Phone);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Phones/Index");
        }
    }
}
