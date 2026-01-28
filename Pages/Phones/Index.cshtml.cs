using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using Microsoft.AspNetCore.Mvc;
using ITInventoryJLS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> OnGetExportCsvAsync()
        {
            var query = _context.Phones.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(ManufacturerFilter))
            {
                query = query.Where(p => p.Manufacturer == ManufacturerFilter);
            }

            var phones = await query.OrderBy(p => p.Manufacturer).ThenBy(p => p.Model).ToListAsync();

            var sb = new StringBuilder();
            // header
            sb.AppendLine("PhoneID,SerialNumber,Manufacturer,Model,Condition,MACAddress,DeploymentLocation");

            foreach (var p in phones)
            {
                string Escape(string? v) => v == null ? "" : $"\"{v.Replace("\"","\"\"")}\"";
                sb.AppendLine(string.Join(",",
                    p.PhoneID.ToString(),
                    Escape(p.SerialNumber),
                    Escape(p.Manufacturer),
                    Escape(p.Model),
                    Escape(p.Condition),
                    Escape(p.MACAddress),
                    Escape(p.DeploymentLocation)));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv; charset=utf-8", "phones.csv");
        }
    }
}
