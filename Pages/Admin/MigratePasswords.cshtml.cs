using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ITInventoryJLS.Data;
using ITInventoryJLS.Services;

namespace ITInventoryJLS.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class MigratePasswordsModel : PageModel
    {
        private readonly AppDbContext _db;

        public MigratePasswordsModel(AppDbContext db)
        {
            _db = db;
        }

        public class UserRow
        {
            public int DBUserID { get; set; }
            public string EmailAddress { get; set; } = string.Empty;
            public string PlainPassword { get; set; } = string.Empty;
            public bool Selected { get; set; }
        }

        public bool DbPasswordColumnExists { get; set; }

        public List<UserRow> Users { get; set; } = new List<UserRow>();

        [BindProperty]
        public List<int> SelectedUserIds { get; set; } = new List<int>();

        public string StatusMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            DbPasswordColumnExists = false;
            var conn = _db.Database.GetDbConnection();
            await using (conn)
            {
                if (conn.State != ConnectionState.Open) await conn.OpenAsync();

                // Check for DBPassword column
                var cmdCheck = conn.CreateCommand();
                cmdCheck.CommandText = @"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DBUsers' AND COLUMN_NAME = 'DBPassword'";
                var exists = await cmdCheck.ExecuteScalarAsync();
                if (exists == null)
                {
                    DbPasswordColumnExists = false;
                    return;
                }

                DbPasswordColumnExists = true;

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT DBUserID, EmailAddress, DBPassword FROM DBUsers WHERE DBPassword IS NOT NULL AND LTRIM(RTRIM(DBPassword)) <> '' ORDER BY EmailAddress";

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(0);
                    var email = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    var pwd = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                    Users.Add(new UserRow { DBUserID = id, EmailAddress = email, PlainPassword = pwd });
                }
            }
        }

        public async Task<IActionResult> OnPostMigrateSelectedAsync()
        {
            if (SelectedUserIds == null || SelectedUserIds.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No users selected.");
                await LoadUsersAsync();
                return Page();
            }

            var conn = _db.Database.GetDbConnection();
            await using (conn)
            {
                if (conn.State != ConnectionState.Open) await conn.OpenAsync();

                // Confirm column exists
                var cmdCheck = conn.CreateCommand();
                cmdCheck.CommandText = @"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DBUsers' AND COLUMN_NAME = 'DBPassword'";
                var exists = await cmdCheck.ExecuteScalarAsync();
                if (exists == null)
                {
                    ModelState.AddModelError(string.Empty, "DBPassword column not found; nothing to migrate.");
                    await LoadUsersAsync();
                    return Page();
                }

                int migrated = 0;
                foreach (var id in SelectedUserIds)
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT DBPassword FROM DBUsers WHERE DBUserID = @id";
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@id";
                    p.Value = id;
                    cmd.Parameters.Add(p);

                    var pwdObj = await cmd.ExecuteScalarAsync();
                    if (pwdObj == null) continue;
                    var pwd = pwdObj.ToString() ?? string.Empty;

                    // Hash and save via EF to update mapped properties
                    var user = await _db.DBUsers.FindAsync(id);
                    if (user == null) continue;

                    PasswordHasher.CreateHash(pwd, out var hash, out var salt);
                    user.PasswordHash = hash;
                    user.PasswordSalt = salt;
                    migrated++;
                }

                await _db.SaveChangesAsync();

                StatusMessage = $"Migrated {migrated} users.";
            }

            await LoadUsersAsync();
            return Page();
        }
    }
}
