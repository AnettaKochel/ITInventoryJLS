using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;

namespace ITInventoryJLS.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;

        public LoginModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Lockout policy
            const int maxFailed = 5;
            const int lockoutMinutes = 15;

            var user = await _db.DBUsers.FirstOrDefaultAsync(u => u.EmailAddress == Input.Email && u.IsActive);

            if (user == null)
            {
                ErrorMessage = "Invalid credentials or inactive account.";
                return Page();
            }

            // Check lockout
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                ErrorMessage = $"Account locked until {user.LockoutEnd.Value.UtcDateTime:yyyy-MM-dd HH:mm} UTC.";
                return Page();
            }

            var verified = ITInventoryJLS.Services.PasswordHasher.Verify(Input.Password, user.PasswordHash, user.PasswordSalt);
            if (!verified)
            {
                // Increment failed attempts and apply lockout if threshold reached
                user.FailedLoginCount = (user.FailedLoginCount < 0 ? 0 : user.FailedLoginCount) + 1;
                if (user.FailedLoginCount >= maxFailed)
                {
                    user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(lockoutMinutes);
                    user.FailedLoginCount = 0; // reset after applying lockout
                }

                await _db.SaveChangesAsync();

                ErrorMessage = "Invalid credentials or inactive account.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim("UserRights", user.UserRights ?? string.Empty)
            };

            // Add role claims so ASP.NET authorization can use them. Assume comma-separated roles in UserRights.
            if (!string.IsNullOrEmpty(user.UserRights))
            {
                var roles = user.UserRights.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            // Successful login: reset failed count and lockout
            user.FailedLoginCount = 0;
            user.LockoutEnd = null;
            await _db.SaveChangesAsync();

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }

            return LocalRedirect("/");
        }
    }
}
