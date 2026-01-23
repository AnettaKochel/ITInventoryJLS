using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;
using System;
using ITInventoryJLS.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure forwarded headers to correctly capture client IPs when behind a proxy/load balancer.
// IMPORTANT: set KnownProxies or KnownNetworks in production to avoid IP spoofing.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // Load known proxies from configuration (appsettings.json)
    try
    {
        var knownProxies = builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>();
        if (knownProxies != null)
        {
            foreach (var p in knownProxies)
            {
                if (System.Net.IPAddress.TryParse(p, out var ip))
                {
                    options.KnownProxies.Add(ip);
                }
            }
        }

        var knownNetworks = builder.Configuration.GetSection("ForwardedHeaders:KnownNetworks").Get<string[]>();
        if (knownNetworks != null)
        {
            foreach (var n in knownNetworks)
            {
                // Expect CIDR like "10.0.0.0/24"
                var parts = n?.Split('/');
                if (parts != null && parts.Length == 2 && System.Net.IPAddress.TryParse(parts[0], out var networkIp) && int.TryParse(parts[1], out var prefix))
                {
                    options.KnownNetworks.Add(new IPNetwork(networkIp, prefix));
                }
            }
        }
    }
    catch
    {
        // If config is malformed, fall back to no trusted proxies to avoid spoofing
    }
});

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Logout");
    options.Conventions.AllowAnonymousToPage("/Error");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Privacy");
    options.Conventions.AllowAnonymousToPage("/Introduction");
});

// Add cookie authentication for simple login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.Cookie.Name = "ITInventoryAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Require authenticated users by default (pages must opt-out with AllowAnonymous)
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Register DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
// Rate limiting: per-IP partitioning with a stricter limit on the login endpoint
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anon";
        // Stricter limits for the login page
        if (httpContext.Request.Path.StartsWithSegments("/Account/Login", StringComparison.OrdinalIgnoreCase))
        {
            return RateLimitPartition.GetFixedWindowLimiter(ip + ":login", _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
        }

        // General per-IP limit
        return RateLimitPartition.GetFixedWindowLimiter(ip + ":general", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.Headers["Retry-After"] = "60";
        await context.HttpContext.Response.WriteAsync("Too many requests. Try again later.", token);
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Important: Forwarded headers must run before other middleware that consumes client IP.
app.UseForwardedHeaders();

app.UseAuthentication();
app.UseAuthorization();

// Enable rate limiting middleware
app.UseRateLimiter();

// Security headers: set a conservative baseline to reduce information leakage and clickjacking.
app.Use(async (context, next) =>
{
    // Prevent MIME type sniffing
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";

    // Prevent clickjacking
    context.Response.Headers["X-Frame-Options"] = "DENY";

    // Basic Referrer Policy
    context.Response.Headers["Referrer-Policy"] = "no-referrer-when-downgrade";

    // XSS protection (legacy browsers)
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

    await next();
});

// Middleware: set AppDbContext.CurrentUser from the authenticated user for each request
app.Use(async (context, next) =>
{
    try
    {
        var db = context.RequestServices.GetService<ITInventoryJLS.Data.AppDbContext>();
        if (db != null)
        {
            var userName = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "Anonymous";
            db.CurrentUser = userName ?? "Anonymous";
        }
    }
    catch
    {
        // ignore errors setting current user
    }

    await next();
});

app.MapRazorPages();

app.Run();
