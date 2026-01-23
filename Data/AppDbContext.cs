using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ITInventoryJLS.Models;

namespace ITInventoryJLS.Data
{
    public class AppDbContext : DbContext
    {
        // Optional: set this before saving to capture who made the change
        public string CurrentUser { get; set; } = "System";

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Waps> Waps { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<DBUser> DBUsers { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        public override int SaveChanges()
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = base.SaveChanges();
            OnAfterSaveChanges(auditEntries).GetAwaiter().GetResult();
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();

            var auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .Where(e => !(e.Entity is AuditLog)))
            {
                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Metadata.GetTableName(),
                    Action = entry.State.ToString(),
                    ChangedBy = CurrentUser,
                    ChangedAt = DateTime.UtcNow
                };

                // If this is a Modified entry, attempt to read the current database values
                // so we can record the real "old" values even when the incoming entity
                // was attached (which would make OriginalValue==CurrentValue).
                //
                // Rationale: some pages use the pattern `Attach(entity); Entry.State = Modified;`
                // which populates the tracked entry with the *new* values only. In that case
                // `prop.OriginalValue` may equal `prop.CurrentValue`. To capture what was
                // actually stored in the database before the change, we call
                // `entry.GetDatabaseValues()` and prefer those values when present.
                // This avoids audit records where OldValues and NewValues are identical.
                var databaseValues = entry.State == EntityState.Modified ? entry.GetDatabaseValues() : null;

                foreach (var prop in entry.Properties)
                {
                    var propName = prop.Metadata.Name;

                    if (prop.Metadata.IsPrimaryKey())
                    {
                        if (prop.IsTemporary)
                        {
                            auditEntry.TemporaryProperties.Add(prop);
                        }
                        else
                        {
                            auditEntry.KeyValues[propName] = RedactValue(propName, prop.CurrentValue);
                        }
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propName] = RedactValue(propName, prop.CurrentValue);
                            break;
                        case EntityState.Deleted:
                            auditEntry.OldValues[propName] = RedactValue(propName, prop.OriginalValue);
                            break;
                        case EntityState.Modified:
                            // Detect changes either by IsModified or value difference
                            if (prop.IsModified || !Equals(prop.OriginalValue, prop.CurrentValue))
                            {
                                // Prefer the database-stored value as the "old" value when available.
                                object? dbOld = null;
                                if (databaseValues != null && databaseValues.Properties.Any(p => p.Name == propName))
                                {
                                    dbOld = databaseValues[propName];
                                }

                                auditEntry.OldValues[propName] = RedactValue(propName, dbOld ?? prop.OriginalValue);
                                auditEntry.NewValues[propName] = RedactValue(propName, prop.CurrentValue);
                            }
                            break;
                    }
                }

                auditEntries.Add(auditEntry);
            }

            // For entries without temporary properties, persist immediately
            foreach (var a in auditEntries.Where(a => !a.HasTemporaryProperties))
            {
                AuditLogs.Add(a.ToAuditLog());
            }

            return auditEntries;
        }

        private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return;

            var entriesWithTemp = auditEntries.Where(a => a.HasTemporaryProperties).ToList();
            if (entriesWithTemp.Count == 0)
                return;

            foreach (var auditEntry in entriesWithTemp)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = RedactValue(prop.Metadata.Name, prop.CurrentValue);
                    }
                }

                AuditLogs.Add(auditEntry.ToAuditLog());
            }

            await base.SaveChangesAsync();
        }

        private class AuditEntry
        {
            public AuditEntry(EntityEntry entry)
            {
                Entry = entry;
                KeyValues = new Dictionary<string, object?>();
                OldValues = new Dictionary<string, object?>();
                NewValues = new Dictionary<string, object?>();
                TemporaryProperties = new List<PropertyEntry>();
            }

            public EntityEntry Entry { get; }
            public string? TableName { get; set; }
            public string? Action { get; set; }
            public Dictionary<string, object?> KeyValues { get; }
            public Dictionary<string, object?> OldValues { get; }
            public Dictionary<string, object?> NewValues { get; }
            public List<PropertyEntry> TemporaryProperties { get; }
            public string? ChangedBy { get; set; }
            public DateTime ChangedAt { get; set; }

            public bool HasTemporaryProperties => TemporaryProperties.Any();

            public AuditLog ToAuditLog()
            {
                return new AuditLog
                {
                    TableName = TableName,
                    Action = Action,
                    ChangedBy = ChangedBy,
                    ChangedAt = ChangedAt,
                    KeyValues = KeyValues.Count == 0 ? null : JsonSerializer.Serialize(KeyValues),
                    OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
                    NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues)
                };
            }
        }

        // Redaction helpers to avoid storing sensitive values in audit logs
        private static readonly string[] SensitiveSubstrings = new[] { "password", "salt", "hash", "secret", "token" };

        private static bool IsSensitiveName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            foreach (var sub in SensitiveSubstrings)
            {
                if (name.IndexOf(sub, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            }
            return false;
        }

        private static object? RedactValue(string propName, object? value)
        {
            if (value == null) return null;
            if (IsSensitiveName(propName)) return "[REDACTED]";
            return value;
        }
    }
}
