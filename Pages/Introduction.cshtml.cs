using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITInventoryJLS.Pages;

public class IntroductionModel : PageModel
{
    [BindProperty]
    public ProductInput Input { get; set; } = new();

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        // TODO: save Input to database
        return RedirectToPage("Index");
    }
}

public class ProductInput
{}