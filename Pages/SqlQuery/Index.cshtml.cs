using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using ITInventoryJLS.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ITInventoryJLS.Pages.SqlQuery
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Query { get; set; } = string.Empty;

        public List<string> Columns { get; set; } = new List<string>();
        public List<List<string>> Rows { get; set; } = new List<List<string>>();
        public string? ErrorMessage { get; set; }

        // Only allow simple SELECT queries. Basic safety checks applied.
        public async Task OnPostAsync()
        {
            Columns.Clear();
            Rows.Clear();
            ErrorMessage = null;

            var q = (Query ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(q))
            {
                ErrorMessage = "Query is empty.";
                return;
            }

            // Remove trailing semicolon if present
            if (q.EndsWith(";")) q = q.Substring(0, q.Length - 1).TrimEnd();

            // Basic validation: must start with SELECT
            var firstToken = q.Split(new[] { ' ', '\t', '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.ToUpperInvariant();
            if (firstToken != "SELECT")
            {
                ErrorMessage = "Only SELECT statements are allowed.";
                return;
            }

            // Disallow dangerous keywords
            var forbidden = new[] { "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "CREATE", "TRUNCATE", "EXEC", "MERGE", "BACKUP", "RESTORE", "GRANT", "REVOKE", "ATTACH", "DETACH", ";--", "--", "/*", "*/" };
            var qUpper = q.ToUpperInvariant();
            foreach (var term in forbidden)
            {
                if (qUpper.Contains(term))
                {
                    ErrorMessage = "Query contains forbidden keywords or comments.";
                    return;
                }
            }

            // Execute using a raw DbCommand but through the configured DbConnection
            try
            {
                var conn = _context.Database.GetDbConnection();
                var openedHere = false;
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                    openedHere = true;
                }

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = q;
                cmd.CommandType = System.Data.CommandType.Text;

                await using var reader = await cmd.ExecuteReaderAsync();

                // Read columns
                var fieldCount = reader.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    Columns.Add(reader.GetName(i));
                }

                // Read rows
                while (await reader.ReadAsync())
                {
                    var row = new List<string>(fieldCount);
                    for (int i = 0; i < fieldCount; i++)
                    {
                        var val = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i)?.ToString() ?? string.Empty;
                        row.Add(val);
                    }
                    Rows.Add(row);
                }

                if (openedHere && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (DbException ex)
            {
                ErrorMessage = "Database error: " + ex.Message;
            }
            catch (System.Exception ex)
            {
                ErrorMessage = "Error: " + ex.Message;
            }
        }
    }
}
