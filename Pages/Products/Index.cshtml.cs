using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITInventoryJLS.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        //public IList<Product> Products { get; set; } = new List<Product>();
        public IList<Waps> Waps { get; set; } = new List<Waps>();

        [BindProperty(SupportsGet = true)]
        public string? DeviceNameFilter { get; set; }

        public List<SelectListItem> DeviceNameOptions { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string? DeploymentLocationFilter { get; set; }

        public List<SelectListItem> DeploymentLocationOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            // populate device name options (distinct values)
            DeviceNameOptions = await _context.Waps
                .AsNoTracking()
                .Select(w => w.DeviceName)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .OrderBy(n => n)
                .Select(n => new SelectListItem { Value = n, Text = n })
                .ToListAsync();

            var query = _context.Waps.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(DeviceNameFilter))
            {
                var s = DeviceNameFilter.Trim().ToLower();
                query = query.Where(w => w.DeviceName.ToLower().Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(DeploymentLocationFilter))
            {
                var s2 = DeploymentLocationFilter.Trim().ToLower();
                query = query.Where(w => w.DeploymentLocation.ToLower().Contains(s2));
            }

            Waps = await query.OrderBy(w => w.DeviceName).ToListAsync();
        }
       
        
    }
}