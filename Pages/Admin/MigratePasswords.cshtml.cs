using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITInventoryJLS.Pages.Admin
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class MigratePasswordsModel : PageModel
    {
        public IActionResult OnGet()
        {
            return NotFound();
        }

        public IActionResult OnPostMigrateSelected()
        {
            return NotFound();
        }
    }
}
