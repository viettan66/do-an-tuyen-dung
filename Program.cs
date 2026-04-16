using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using JobBoard.Services;
using JobBoard.Models;

var builder = WebApplication.CreateBuilder(args);

// If running on Render (or other PaaS) the platform provides a PORT env var.
// Bind Kestrel to that port so Render can route traffic correctly.
var portEnv = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(portEnv))
{
    // Listen on all network interfaces on the provided port
    builder.WebHost.UseUrls($"http://0.0.0.0:{portEnv}");
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<MockDataService>();
builder.Services.AddHttpContextAccessor();

// HttpClient for components to call minimal API endpoints
// No BaseAddress = requests go to current host (localhost in dev, render.com in prod)
builder.Services.AddScoped(sp => new HttpClient());

// Cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "JobBoardAuth";
        options.ExpireTimeSpan = TimeSpan.FromDays(2);
        options.SlidingExpiration = false;
        options.LoginPath = "/login";
        options.Cookie.HttpOnly = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/login", async (LoginRequest req, MockDataService data, HttpContext http, ILogger<Program> logger) =>
{
    try
    {
        if (req == null)
        {
            logger.LogWarning("Login request is null");
            return Results.BadRequest();
        }
        
        logger.LogInformation($"Login attempt: username={req.Username}");
        var user = data.ValidateUser(req.Username ?? string.Empty, req.Password ?? string.Empty);
        if (user == null)
        {
            logger.LogWarning($"Login failed: user not found or invalid password for username={req.Username}");
            return Results.Unauthorized();
        }

        logger.LogInformation($"Login success: username={user.Username}, displayName={user.DisplayName}");
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("DisplayName", user.DisplayName ?? user.Username)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var props = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2)
        };
        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
        return Results.Ok(new { displayName = user.DisplayName });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unexpected error during login");
        return Results.StatusCode(500);
    }
});

app.MapPost("/api/auth/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

