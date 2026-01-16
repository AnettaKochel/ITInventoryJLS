using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITInventoryJLS.Pages.Search
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true, Name = "q")]
        public string? Query { get; set; }

        public List<Location> LocationResults { get; set; } = new();
        public List<Waps> WapsResults { get; set; } = new();
        public List<Phone> PhoneResults { get; set; } = new();
        public List<Product> ProductResults { get; set; } = new();
        public List<Employee> EmployeeResults { get; set; } = new();
        public List<Computer> ComputerResults { get; set; } = new();

        public int TotalResults => LocationResults.Count + WapsResults.Count + PhoneResults.Count + ProductResults.Count + EmployeeResults.Count + ComputerResults.Count;

        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                return;
            }

            var s = Query.Trim();
            var sLower = s.ToLower();

            // Search Locations
            LocationResults = await _context.Locations
                .AsNoTracking()
                .Where(l => l.LocationName.ToLower().Contains(sLower)
                         || l.LocationAccountingID.ToLower().Contains(sLower)
                         || l.StreetAddress.ToLower().Contains(sLower)
                         || l.CityStateZip.ToLower().Contains(sLower)
                         || (l.FWIP ?? string.Empty).ToLower().Contains(sLower))
                .OrderBy(l => l.LocationName)
                .ToListAsync();

            // Search Waps
            WapsResults = await _context.Waps
                .AsNoTracking()
                .Where(w => w.SerialNumber.ToLower().Contains(sLower)
                         || w.Manufacturer.ToLower().Contains(sLower)
                         || w.Model.ToLower().Contains(sLower)
                         || w.Condition.ToLower().Contains(sLower)
                         || w.DeploymentLocation.ToLower().Contains(sLower))
                .OrderBy(w => w.Manufacturer)
                .ToListAsync();

            // Search Products
            ProductResults = await _context.Products
                .AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(sLower))
                .OrderBy(p => p.Name)
                .ToListAsync();

            // Search Employees
            EmployeeResults = await _context.Employees
                .AsNoTracking()
                .Where(e => e.FullName.ToLower().Contains(sLower)
                         || e.FirstName.ToLower().Contains(sLower)
                         || e.LastName.ToLower().Contains(sLower)
                         || e.Department.ToLower().Contains(sLower)
                         || e.Title.ToLower().Contains(sLower)
                         || e.OfficeName.ToLower().Contains(sLower)
                         || e.State.ToLower().Contains(sLower)
                         || e.Email.ToLower().Contains(sLower))
                .OrderBy(e => e.FullName)
                .ToListAsync();

            // Search Computers
            ComputerResults = await _context.Computers
                .AsNoTracking()
                .Where(c => c.DeviceName.ToLower().Contains(sLower)
                         || c.OSVersionNumber.ToLower().Contains(sLower)
                         || c.OSVersion.ToLower().Contains(sLower)
                         || c.SerialNumber.ToLower().Contains(sLower)
                         || c.Manufacturer.ToLower().Contains(sLower)
                         || c.Model.ToLower().Contains(sLower)
                         || c.Type.ToLower().Contains(sLower)
                         || c.DeploymentLocation.ToLower().Contains(sLower)
                         || c.PrimaryUserEmailAddress.ToLower().Contains(sLower)
                         || c.PrimaryUserDisplayName.ToLower().Contains(sLower)
                         || c.ManagedBy.ToLower().Contains(sLower)
                         || c.JoinType.ToLower().Contains(sLower)
                         || c.DeviceStatus.ToLower().Contains(sLower))
                .OrderBy(c => c.DeviceName)
                .ToListAsync();

            // Search Phones
            PhoneResults = await _context.Phones
                .AsNoTracking()
                .Where(p => p.SerialNumber.ToLower().Contains(sLower)
                         || p.Manufacturer.ToLower().Contains(sLower)
                         || p.Model.ToLower().Contains(sLower)
                         || p.Condition.ToLower().Contains(sLower)
                         || p.MACAddress.ToLower().Contains(sLower)
                         || p.DeploymentLocation.ToLower().Contains(sLower))
                .OrderBy(p => p.Manufacturer)
                .ThenBy(p => p.Model)
                .ToListAsync();
        }
    }
}
