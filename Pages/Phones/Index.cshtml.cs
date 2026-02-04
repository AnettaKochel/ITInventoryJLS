using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using Microsoft.AspNetCore.Mvc;
using ITInventoryJLS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

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

        [BindProperty(SupportsGet = true)]
        public string? MACFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DeploymentLocationFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? UserAssignedFilter { get; set; }

        public List<string> MACOptions { get; set; } = new List<string>();
        public List<string> DeploymentLocationOptions { get; set; } = new List<string>();
        public List<string> UserAssignedOptions { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            var query = _context.Phones.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(ManufacturerFilter))
            {
                query = query.Where(p => p.Manufacturer == ManufacturerFilter);
            }

            var macFilter = string.IsNullOrWhiteSpace(MACFilter) ? null : MACFilter.Trim();
            var deploymentFilter = string.IsNullOrWhiteSpace(DeploymentLocationFilter) ? null : DeploymentLocationFilter.Trim();
            var userFilter = string.IsNullOrWhiteSpace(UserAssignedFilter) ? null : UserAssignedFilter.Trim();

            if (macFilter != null)
            {
                query = query.Where(p => p.MACAddress.Trim() == macFilter);
            }

            if (deploymentFilter != null)
            {
                query = query.Where(p => p.DeploymentLocation.Trim() == deploymentFilter);
            }

            if (userFilter != null)
            {
                query = query.Where(p => p.UserAssigned.Trim() == userFilter);
            }

            // Populate dropdown options from existing Phones table values
            MACOptions = await _context.Phones
                .AsNoTracking()
                .Where(p => !string.IsNullOrEmpty(p.MACAddress))
                .Select(p => p.MACAddress.Trim())
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            DeploymentLocationOptions = await _context.Phones
                .AsNoTracking()
                .Where(p => !string.IsNullOrEmpty(p.DeploymentLocation))
                .Select(p => p.DeploymentLocation.Trim())
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            UserAssignedOptions = await _context.Phones
                .AsNoTracking()
                .Where(p => !string.IsNullOrEmpty(p.UserAssigned))
                .Select(p => p.UserAssigned.Trim())
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            Phones = await query.OrderBy(p => p.Manufacturer).ThenBy(p => p.Model).ToListAsync();
        }

        public async Task<IActionResult> OnGetExportCsvAsync()
        {
            var query = _context.Phones.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(ManufacturerFilter))
            {
                query = query.Where(p => p.Manufacturer == ManufacturerFilter);
            }

            var macFilter2 = string.IsNullOrWhiteSpace(MACFilter) ? null : MACFilter.Trim();
            var deploymentFilter2 = string.IsNullOrWhiteSpace(DeploymentLocationFilter) ? null : DeploymentLocationFilter.Trim();
            var userFilter2 = string.IsNullOrWhiteSpace(UserAssignedFilter) ? null : UserAssignedFilter.Trim();

            if (macFilter2 != null)
            {
                query = query.Where(p => p.MACAddress == macFilter2);
            }

            if (deploymentFilter2 != null)
            {
                query = query.Where(p => p.DeploymentLocation == deploymentFilter2);
            }

            if (userFilter2 != null)
            {
                query = query.Where(p => p.UserAssigned == userFilter2);
            }

            var phones = await query.OrderBy(p => p.Manufacturer).ThenBy(p => p.Model).ToListAsync();

            var sb = new StringBuilder();
            // header
            sb.AppendLine("PhoneID,SerialNumber,Manufacturer,Model,Condition,UserAssigned,MACAddress,DeploymentLocation");

            foreach (var p in phones)
            {
                string Escape(string? v) => v == null ? "" : $"\"{v.Replace("\"","\"\"")}\"";
                sb.AppendLine(string.Join(",",
                    p.PhoneID.ToString(),
                    Escape(p.SerialNumber),
                    Escape(p.Manufacturer),
                    Escape(p.Model),
                    Escape(p.Condition),
                    Escape(p.UserAssigned),
                    Escape(p.MACAddress),
                    Escape(p.DeploymentLocation)));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv; charset=utf-8", "phones.csv");
        }
    }
}
