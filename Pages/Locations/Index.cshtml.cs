using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITInventoryJLS.Pages.Locations
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Location> Locations { get; set; } = new List<Location>();

        public string SortOrder { get; set; } = "A → Z";


        public async Task OnGetAsync()
        {
            // Fetch all Locations from the database, ordered by LocationName ascending
            Locations = await _context.Locations
                .AsNoTracking()
                .OrderBy(l => l.LocationName)
                .ToListAsync();

            SortOrder = "A → Z";
        }
       
        
    }
}