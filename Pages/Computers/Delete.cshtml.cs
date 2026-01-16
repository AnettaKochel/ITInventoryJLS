using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;

namespace ITInventoryJLS.Pages.Computers
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Computer Computer { get; set; } = new Computer();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var computerFromDb = await _context.Computers.FindAsync(id);
            if (computerFromDb == null) return NotFound();

            Computer = computerFromDb;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Computer == null || Computer.DeviceID == 0) return NotFound();

            var comp = await _context.Computers.FindAsync(Computer.DeviceID);

            if (comp != null)
            {
                _context.Computers.Remove(comp);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Computers/Index");
        }
    }
}
