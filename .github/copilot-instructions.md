<!-- Copilot instructions for AI coding agents working on ITInventoryJLS -->
# Copilot instructions — ITInventoryJLS
```markdown
# Copilot instructions — ITInventoryJLS

Purpose: concise, repo-specific guidance so AI coding agents are productive in this ASP.NET Core Razor Pages + EF Core app.

## Project summary
- Framework: ASP.NET Core Razor Pages targeting .NET 8 (see `ITInventoryJLS/Program.cs`).
- Data: EF Core `AppDbContext` (SQL Server) configured from `appsettings.json` `ConnectionStrings:DefaultConnection`.
- Auth: Cookie authentication (`ITInventoryAuth`) with 8-hour sliding expiration. Pages default to RequireAuthenticatedUser; anonymous pages are explicitly allowed in `Program.cs`.
- Audit: `AppDbContext` overrides `SaveChanges`/`SaveChangesAsync` to capture AuditLog entries (Old/New values, who changed them). Middleware sets `AppDbContext.CurrentUser` from the authenticated user.

## Key files and locations
- `ITInventoryJLS/Program.cs` — startup, DI, cookie auth, security headers, middleware that populates `AppDbContext.CurrentUser`.
- `ITInventoryJLS/Data/AppDbContext.cs` — DbSets (Products, Computers, DBUsers, AuditLogs), audit implementation and temporary property handling.
- `ITInventoryJLS/Services/PasswordHasher.cs` — PBKDF2 (HMACSHA256) at 100,000 iterations; used for DB user password hashing and verification.
- `ITInventoryJLS/Pages/` — Razor Pages organized per entity (e.g., `Pages/Computers/`), with `.cshtml` + `.cshtml.cs` pairing.
- `ITInventoryJLS/Migrations/` — EF migrations when present; review before adding new migrations.
- `Tools/` — small command-line helpers (e.g., `MigratePlaintextPasswords`) useful for bulk data operations.

## Commands and workflows
- Build: run from workspace root or project folder: `dotnet build ITInventoryJLS/ITInventoryJLS.csproj` or from inside `ITInventoryJLS` run `dotnet build`.
- Run locally: `dotnet run --project ITInventoryJLS/ITInventoryJLS.csproj` or `cd ITInventoryJLS && dotnet run`.
- Tests: `dotnet test ITInventoryJLS.Tests/ITInventoryJLS.Tests.csproj`.
- EF Core (from workspace root):
  - Install tools: `dotnet tool install --global dotnet-ef`
  - Add migration: `dotnet ef migrations add <Name> --project ITInventoryJLS/ITInventoryJLS.csproj --startup-project ITInventoryJLS/ITInventoryJLS.csproj`
  - Apply migrations: `dotnet ef database update --project ITInventoryJLS/ITInventoryJLS.csproj --startup-project ITInventoryJLS/ITInventoryJLS.csproj`

## Important repo-specific conventions
- Razor Page pattern: New features should add a folder under `Pages/<Feature>/` with `Index.cshtml`, `Create.cshtml`, `Edit.cshtml` and matching `*.cshtml.cs` PageModel classes.
- Data annotations are used for validation on models in `Models/` (follow existing `[Required]`, `[StringLength]`, `[EmailAddress]`).
- When updating models that affect the DB schema, create EF migrations and confirm `Migrations/` contents are correct before committing.
- Password handling: use `Services/PasswordHasher` for creating/verifying password hashes. The stored DB fields are base64-encoded salt and hash (no iteration metadata stored currently).

## Security notes and guardrails
- Do not add ad-hoc SQL execution endpoints (see prior `Pages/SqlQuery` history). If an admin query tool is necessary, restrict it to Admin-only pages, sanitize/limit queries, or run as maintenance scripts only.
- Cookie settings use `CookieSecurePolicy.Always` and `HttpOnly`; preserve these defaults when changing auth behavior.

## Examples (concrete references)
- Capture current user in middleware: inspect the middleware block in `Program.cs` that sets `db.CurrentUser`.
- Audit behavior: see `OnBeforeSaveChanges()` and `OnAfterSaveChanges()` in `Data/AppDbContext.cs` for how Old/New values and temporary PKs are handled.
- Password hashing usage: `Services/PasswordHasher.CreateHash(...)` and `Verify(...)`.

## When to ask the human
- Any change that touches DB schema (ask whether to add a migration and run it).
- Any change that exposes new query execution from UI (ask about security and intended audience).

If something seems ambiguous, request the exact intended user-visible behavior, whether a DB migration is expected, and if the change should include UI updates or tests.
```
