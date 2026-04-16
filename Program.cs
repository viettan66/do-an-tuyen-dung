using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using JobBoard.Services;
using JobBoard.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<MockDataService>();
builder.Services.AddHttpContextAccessor();

// HttpClient for components to call minimal API endpoints
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["BaseUrl"] ?? "http://localhost:5000") });

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

app.MapPost("/api/auth/login", async (LoginRequest req, MockDataService data, HttpContext http) =>
{
    if (req == null) return Results.BadRequest();
    var user = data.ValidateUser(req.Username ?? string.Empty, req.Password ?? string.Empty);
    if (user == null) return Results.Unauthorized();

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
});

app.MapPost("/api/auth/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

