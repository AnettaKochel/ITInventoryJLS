using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;

namespace ITInventoryJLS.Pages.Employees
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; } = new Employee();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var employeeFromDb = await _context.Employees.FindAsync(id);
            if (employeeFromDb == null) return NotFound();

            Employee = employeeFromDb;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Employee == null || Employee.EmployeeID == 0) return NotFound();

            var employee = await _context.Employees.FindAsync(Employee.EmployeeID);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Employees/Index");
        }
    }
}
