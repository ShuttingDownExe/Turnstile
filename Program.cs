using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = "/Verify";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HumanVerified", policy =>
    {
        policy.RequireClaim("HumanVerified","true");
    });
});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.Configure<TurnstileOptions>(
    builder.Configuration.GetSection("CloudflareTurnstile"));

builder.Services.AddHttpClient("Turnstile", client =>
{
    client.BaseAddress = new Uri("https://challenges.cloudflare.com");
});

builder.Services.AddScoped<TurnstileValidator>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.Run();


public class TurnstileOptions
{
    public string SiteKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}
