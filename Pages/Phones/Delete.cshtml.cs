using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;

namespace ITInventoryJLS.Pages.Phones
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Phone Phone { get; set; } = new Phone();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var phoneFromDb = await _context.Phones.FindAsync(id);
            if (phoneFromDb == null) return NotFound();

            Phone = phoneFromDb;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Phone == null || Phone.PhoneID == 0) return NotFound();

            var ph = await _context.Phones.FindAsync(Phone.PhoneID);

            if (ph != null)
            {
                _context.Phones.Remove(ph);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Phones/Index");
        }
    }
}
