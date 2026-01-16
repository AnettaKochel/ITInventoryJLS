using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITInventoryJLS.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Employee> Employees { get; set; } = new List<Employee>();

        public string CurrentSort { get; set; } = string.Empty;
        public string FullNameSort { get; set; } = string.Empty;
        public string TitleSort { get; set; } = string.Empty;
        public string DepartmentSort { get; set; } = string.Empty;
        public string OfficeNameSort { get; set; } = string.Empty;

        public string SortOrder { get; set; } = "A → Z";

        public async Task OnGetAsync(string sortOrder)
        {
            CurrentSort = sortOrder;

            FullNameSort = string.IsNullOrEmpty(sortOrder) || sortOrder == "fullname" ? "fullname_desc" : "fullname";
            TitleSort = sortOrder == "title" ? "title_desc" : "title";
            DepartmentSort = sortOrder == "department" ? "department_desc" : "department";
            OfficeNameSort = sortOrder == "officename" ? "officename_desc" : "officename";

            var employeesIQ = _context.Employees.AsNoTracking();

            switch (sortOrder)
            {
                case "fullname_desc":
                    employeesIQ = employeesIQ.OrderByDescending(e => e.FullName);
                    break;
                case "title":
                    employeesIQ = employeesIQ.OrderBy(e => e.Title);
                    break;
                case "title_desc":
                    employeesIQ = employeesIQ.OrderByDescending(e => e.Title);
                    break;
                case "department":
                    employeesIQ = employeesIQ.OrderBy(e => e.Department);
                    break;
                case "department_desc":
                    employeesIQ = employeesIQ.OrderByDescending(e => e.Department);
                    break;
                case "officename":
                    employeesIQ = employeesIQ.OrderBy(e => e.OfficeName);
                    break;
                case "officename_desc":
                    employeesIQ = employeesIQ.OrderByDescending(e => e.OfficeName);
                    break;
                default:
                    employeesIQ = employeesIQ.OrderBy(e => e.FullName);
                    break;
            }

            Employees = await employeesIQ.ToListAsync();

            SortOrder = sortOrder == "fullname_desc" ? "Z → A" : "A → Z";
        }
    }
}
