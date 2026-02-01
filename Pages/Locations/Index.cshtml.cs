using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Text;
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


        [BindProperty(SupportsGet = true)]
        public string? Export { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch all Locations from the database, ordered by LocationName ascending
            Locations = await _context.Locations
                .AsNoTracking()
                .OrderBy(l => l.LocationName)
                .ToListAsync();

            SortOrder = "A → Z";

            if (!string.IsNullOrWhiteSpace(Export) && Export.ToLower() == "csv")
            {
                var sb = new StringBuilder();
                string esc(string? s)
                {
                    if (s == null) return "";
                    if (s.Contains('"')) s = s.Replace("\"", "\"\"");
                    if (s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r'))
                        return '"' + s + '"';
                    return s;
                }

                sb.AppendLine(string.Join(',', new[] { "LocationId","LocationName","LocationAccountingID","StreetAddress","CityStateZip","FWIP" }));

                foreach (var l in Locations)
                {
                    sb.AppendLine(string.Join(',', new[] {
                        esc(l.LocationId.ToString()),
                        esc(l.LocationName),
                        esc(l.LocationAccountingID),
                        esc(l.StreetAddress),
                        esc(l.CityStateZip),
                        esc(l.FWIP)
                    }));
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                return File(bytes, "text/csv", "locations.csv");
            }

            return Page();
        }
       
        
    }
}