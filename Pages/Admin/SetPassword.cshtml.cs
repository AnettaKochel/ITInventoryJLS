using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using Microsoft.AspNetCore.Authorization;
using ITInventoryJLS.Models;

namespace ITInventoryJLS.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class SetPasswordModel : PageModel
    {
        private readonly AppDbContext _db;

        public SetPasswordModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public string SelectedEmail { get; set; } = string.Empty;

        [BindProperty]
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required, DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public SelectList UserOptions { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        public string StatusMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var users = await _db.DBUsers
                .AsNoTracking()
                .OrderBy(u => u.EmailAddress)
                .Select(u => new SelectListItem { Value = u.EmailAddress, Text = u.EmailAddress })
                .ToListAsync();

            UserOptions = new SelectList(users, "Value", "Text");
        }

        public async Task<IActionResult> OnPostSetSingleAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var user = await _db.DBUsers.FirstOrDefaultAsync(u => u.EmailAddress == SelectedEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                await OnGetAsync();
                return Page();
            }

            ITInventoryJLS.Services.PasswordHasher.CreateHash(NewPassword, out var hash, out var salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            await _db.SaveChangesAsync();

            StatusMessage = $"Password updated for {user.EmailAddress}.";
            await OnGetAsync();
            return Page();
        }
    }
}
