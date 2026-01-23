using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;
using System.Collections.Generic;

namespace ITInventoryJLS.Pages.Computers
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Computer Computer { get; set; } = new Computer();

        public List<SelectListItem> LocationOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> EmployeeOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> EmployeeEmailOptions { get; set; } = new List<SelectListItem>();
        public List<SimpleEmployee> EmployeePairs { get; set; } = new List<SimpleEmployee>();

        public class SimpleEmployee
        {
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            LocationOptions = await _context.Locations
                .AsNoTracking()
                .OrderBy(l => l.LocationName)
                .Select(l => new SelectListItem { Value = l.LocationName, Text = l.LocationName })
                .ToListAsync();

            EmployeeOptions = await _context.Employees
                .AsNoTracking()
                .OrderBy(e => e.FullName)
                .Select(e => new SelectListItem { Value = e.FullName, Text = e.FullName })
                .ToListAsync();
            EmployeeEmailOptions = await _context.Employees
                .AsNoTracking()
                .OrderBy(e => e.Email)
                .Select(e => new SelectListItem { Value = e.Email, Text = e.Email })
                .ToListAsync();

            EmployeePairs = await _context.Employees
                .AsNoTracking()
                .Select(e => new SimpleEmployee { FullName = e.FullName, Email = e.Email })
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
                EmployeeOptions = await _context.Employees
                    .AsNoTracking()
                    .OrderBy(e => e.FullName)
                    .Select(e => new SelectListItem { Value = e.FullName, Text = e.FullName })
                    .ToListAsync();
                EmployeeEmailOptions = await _context.Employees
                    .AsNoTracking()
                    .OrderBy(e => e.Email)
                    .Select(e => new SelectListItem { Value = e.Email, Text = e.Email })
                    .ToListAsync();
                EmployeePairs = await _context.Employees
                    .AsNoTracking()
                    .Select(e => new SimpleEmployee { FullName = e.FullName, Email = e.Email })
                    .ToListAsync();
                return Page();
            }

            var exists = await _context.Locations.AnyAsync(l => l.LocationName == Computer.DeploymentLocation);
            if (!exists)
            {
                ModelState.AddModelError("Computer.DeploymentLocation", "Invalid location selected.");
                LocationOptions = await _context.Locations
                    .AsNoTracking()
                    .OrderBy(l => l.LocationName)
                    .Select(l => new SelectListItem { Value = l.LocationName, Text = l.LocationName })
                    .ToListAsync();
                EmployeeOptions = await _context.Employees
                    .AsNoTracking()
                    .OrderBy(e => e.FullName)
                    .Select(e => new SelectListItem { Value = e.FullName, Text = e.FullName })
                    .ToListAsync();
                EmployeeEmailOptions = await _context.Employees
                    .AsNoTracking()
                    .OrderBy(e => e.Email)
                    .Select(e => new SelectListItem { Value = e.Email, Text = e.Email })
                    .ToListAsync();
                EmployeePairs = await _context.Employees
                    .AsNoTracking()
                    .Select(e => new SimpleEmployee { FullName = e.FullName, Email = e.Email })
                    .ToListAsync();
                return Page();
            }

            var userExists = await _context.Employees.AnyAsync(e => e.FullName == Computer.PrimaryUserDisplayName);
            if (!userExists)
            {
                ModelState.AddModelError("Computer.PrimaryUserDisplayName", "Please select a valid primary user from the list.");
                LocationOptions = await _context.Locations
                    .AsNoTracking()
                    .OrderBy(l => l.LocationName)
                    .Select(l => new SelectListItem { Value = l.LocationName, Text = l.LocationName })
                    .ToListAsync();
                EmployeeOptions = await _context.Employees
                    .AsNoTracking()
                    .OrderBy(e => e.FullName)
                    .Select(e => new SelectListItem { Value = e.FullName, Text = e.FullName })
                    .ToListAsync();
                EmployeeEmailOptions = await _context.Employees
                    .AsNoTracking()
                    .OrderBy(e => e.Email)
                    .Select(e => new SelectListItem { Value = e.Email, Text = e.Email })
                    .ToListAsync();
                return Page();
            }

            var emailExists = await _context.Employees.AnyAsync(e => e.Email == Computer.PrimaryUserEmailAddress);
            if (!emailExists)
            {
                ModelState.AddModelError("Computer.PrimaryUserEmailAddress", "Please select a valid primary user email from the list.");
                LocationOptions = await _context.Locations
                    .AsNoTracking()
                    .OrderBy(l => l.LocationName)
                    .Select(l => new SelectListItem { Value = l.LocationName, Text = l.LocationName })
                    .ToListAsync();
                EmployeeOptions = await _context.Employees
                    .AsNoTracking()
                    .OrderBy(e => e.FullName)
                    .Select(e => new SelectListItem { Value = e.FullName, Text = e.FullName })
                    .ToListAsync();
                EmployeeEmailOptions = await _context.Employees
                    .AsNoTracking()
                    .OrderBy(e => e.Email)
                    .Select(e => new SelectListItem { Value = e.Email, Text = e.Email })
                    .ToListAsync();
                return Page();
            }

            _context.Computers.Add(Computer);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Computers/Index");
        }
    }
}
