<!-- Copilot instructions for AI coding agents working on ITInventoryJLS -->
# Copilot instructions — ITInventoryJLS

Purpose: provide short, actionable guidance so an AI coding agent is productive quickly in this Razor Pages + EF Core project.

- **Project type:** ASP.NET Core Razor Pages (net8.0). See [Program.cs](Program.cs) for service registration.
- **Data access:** EF Core `AppDbContext` registered in `Program.cs` using SQL Server. See [Data/AppDbContext.cs](Data/AppDbContext.cs).
- **Connection string:** stored in [appsettings.json](appsettings.json) under `ConnectionStrings:DefaultConnection`.

Key patterns and conventions
- Pages are organized by entity under `Pages/` (example: `Pages/Computers/` contains `Index`, `Create`, `Edit`). Follow the existing PageModel + Razor file pairing pattern when adding features.
- Models live in `Models/` and use data annotations (`[Required]`, `StringLength`, `[Key]`, `EmailAddress`). Use the same attribute-based validation style.
- `DbSet` properties in `AppDbContext` are singular model types with plural property names (e.g., `public DbSet<Computer> Computers { get; set; }`).
- Dates use `DateOnly` in models (be careful when seeding/test data and serializing).

Build / run / debug
- Build: `dotnet build` (root). Run locally: `dotnet run --project ITInventoryJLS.csproj` or use the profile urls in `Properties/launchSettings.json`.
- For iterative development: `dotnet watch run` (install `dotnet-watch` if not present).
- To attach a debugger: run the project with the `https` profile from `launchSettings.json` or launch from the IDE.

Database and migrations
- No Migrations folder is committed. To manage schema use EF tools:
  - `dotnet tool install --global dotnet-ef`
  - `dotnet ef migrations add Initial --project ITInventoryJLS.csproj --startup-project ITInventoryJLS.csproj`
  - `dotnet ef database update --project ITInventoryJLS.csproj --startup-project ITInventoryJLS.csproj`
- Always check `appsettings.json` for the `DefaultConnection` before running migrations.

Security & risky code patterns
- The project previously included an ad-hoc SQL query tool under `Pages/SqlQuery`. That tool has been removed — avoid adding similar functionality because it can enable data exfiltration and risks SQL injection. If a read-only query utility is required, gate it with `Admin` role, run it under least-privileged credentials, and thoroughly limit queries and results.

Code changes guidance
- Preserve Razor Pages structure: add a PageModel class co-located with its `.cshtml` file under `Pages/<Entity>/`.
- When adding a model field, mirror data-annotation validation in the corresponding Create/Edit pages and View templates.
- Keep UI assets in `wwwroot/` and use the existing `lib/` packages (Bootstrap, jQuery). Add new static files under `wwwroot/js` or `wwwroot/css`.

Files worth reading first
- [Program.cs](Program.cs) — app startup and DI.
- [Data/AppDbContext.cs](Data/AppDbContext.cs) — DbSets and DB access surface.
- [Pages/Shared/_Layout.cshtml](Pages/Shared/_Layout.cshtml) — navigation and common UI layout.
- `Pages/<Entity>/Index.cshtml.cs` (e.g., `Pages/Computers/Index.cshtml.cs`) — representative CRUD pattern.

When making changes, ask the user if:
- a migration should be created and applied to the shared development database;
- the `DefaultConnection` should be updated for CI or environment-specific testing.

If anything is unclear, ask for the exact desired behavior and whether the change should include schema migrations and UI updates.
