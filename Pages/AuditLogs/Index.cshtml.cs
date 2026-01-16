using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using ITInventoryJLS.Models;

namespace ITInventoryJLS.Pages.AuditLogs
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        [BindProperty(SupportsGet = true)]
        public string? TableFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 25;

        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public async Task OnGetAsync()
        {
            if (PageNumber < 1) PageNumber = 1;
            if (PageSize < 1 || PageSize > 200) PageSize = 25;

            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(TableFilter))
            {
                var f = TableFilter.Trim();
                query = query.Where(a => a.TableName != null && EF.Functions.Like(a.TableName, "%" + f + "%"));
            }

            TotalCount = await query.CountAsync();
            TotalPages = (int)System.Math.Ceiling(TotalCount / (double)PageSize);

            AuditLogs = await query
                .OrderByDescending(a => a.ChangedAt)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(a => new AuditLog
                {
                    Id = a.Id,
                    Action = a.Action ?? string.Empty,
                    ChangedAt = a.ChangedAt,
                    ChangedBy = a.ChangedBy ?? string.Empty,
                    KeyValues = a.KeyValues ?? string.Empty,
                    NewValues = a.NewValues ?? string.Empty,
                    OldValues = a.OldValues ?? string.Empty,
                    TableName = a.TableName ?? string.Empty
                })
                .ToListAsync();
        }
    }
}
