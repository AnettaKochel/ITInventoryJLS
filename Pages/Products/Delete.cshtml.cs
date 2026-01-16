using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;

namespace ITInventoryJLS.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Waps Waps { get; set; } = new Waps();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var wapsFromDb = await _context.Waps.FindAsync(id);
            if (wapsFromDb == null) return NotFound();

            Waps = wapsFromDb;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Waps == null || Waps.Id == 0) return NotFound();

            var wap = await _context.Waps.FindAsync(Waps.Id);

            if (wap != null)
            {
                _context.Waps.Remove(wap);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Products/Index");
        }
    }
}
