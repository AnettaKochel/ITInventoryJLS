using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public int TotalCount { get; set; }

        public bool IsFiltered => !string.IsNullOrWhiteSpace(TypeFilter)
                      || !string.IsNullOrWhiteSpace(OSFilter)
                      || !string.IsNullOrWhiteSpace(DeploymentLocationFilter);

        [BindProperty(SupportsGet = true)]
        public string? TypeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OSFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DeploymentLocationFilter { get; set; }

        public List<SelectListItem> TypeOptions { get; set; } = new();
        public List<SelectListItem> OSOptions { get; set; } = new();
        public List<SelectListItem> DeploymentLocationOptions { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Export { get; set; }

        public async Task<IActionResult> OnGetAsync()
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

            DeploymentLocationOptions = await _context.Computers
                .AsNoTracking()
                .Select(c => c.DeploymentLocation)
                .Where(d => !string.IsNullOrEmpty(d))
                .Distinct()
                .OrderBy(d => d)
                .Select(d => new SelectListItem { Value = d, Text = d })
                .ToListAsync();

            var query = _context.Computers.AsNoTracking().AsQueryable();

            // total count before applying filters
            TotalCount = await _context.Computers.AsNoTracking().CountAsync();

            if (!string.IsNullOrWhiteSpace(TypeFilter))
            {
                query = query.Where(c => c.Type == TypeFilter);
            }

            if (!string.IsNullOrWhiteSpace(OSFilter))
            {
                query = query.Where(c => c.OSVersion == OSFilter);
            }

            if (!string.IsNullOrWhiteSpace(DeploymentLocationFilter))
            {
                query = query.Where(c => c.DeploymentLocation == DeploymentLocationFilter);
            }

            Computers = await query.OrderBy(c => c.DeviceName).ToListAsync();

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

                sb.AppendLine(string.Join(',', new[] {
                    "DeviceID","DeviceName","OSVersionNumber","OSVersion","SerialNumber","Manufacturer","Model","Type","DeploymentLocation","PrimaryUserEmailAddress","PrimaryUserDisplayName","ManagedBy","JoinType","PurchasedDate","DeployedDate","DeviceStatus"
                }));

                foreach (var c in Computers)
                {
                    sb.AppendLine(string.Join(',', new[] {
                        esc(c.DeviceID.ToString()),
                        esc(c.DeviceName),
                        esc(c.OSVersionNumber),
                        esc(c.OSVersion),
                        esc(c.SerialNumber),
                        esc(c.Manufacturer),
                        esc(c.Model),
                        esc(c.Type),
                        esc(c.DeploymentLocation),
                        esc(c.PrimaryUserEmailAddress),
                        esc(c.PrimaryUserDisplayName),
                        esc(c.ManagedBy),
                        esc(c.JoinType),
                        esc(c.PurchasedDate.ToString()),
                        esc(c.DeployedDate.ToString()),
                        esc(c.DeviceStatus)
                    }));
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                return File(bytes, "text/csv", "computers.csv");
            }

            return Page();
        }
    }
}
