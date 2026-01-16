using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITInventoryJLS.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        //public IList<Product> Products { get; set; } = new List<Product>();
        public IList<Waps> Waps { get; set; } = new List<Waps>();

        public async Task OnGetAsync()
        {
            // Fetch all Waps from the database
            //Products = await _context.Products.AsNoTracking().ToListAsync();
            Waps = await _context.Waps.AsNoTracking().ToListAsync();
        }
       
        
    }
}