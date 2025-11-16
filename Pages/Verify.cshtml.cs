using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

public class VerifyModel : PageModel
{
    private readonly TurnstileValidator _turnstile;
    private readonly TurnstileOptions _options;

    public VerifyModel(TurnstileValidator turnstile, IOptions<TurnstileOptions> options)
    {
        _turnstile = turnstile;
        _options = options.Value;
    }

    public string TurnstileSiteKey => _options.SiteKey;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var token = Request.Form["cf-turnstile-response"].ToString();
        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        var human = await _turnstile.IsValidAsync(token, remoteIp);
        if (!human)
        {
            ModelState.AddModelError(string.Empty, "Verification failed. Please try again.");
            return Page();
        }

        // ✅ Mark user as "verified" using a cookie with a claim
        var claims = new List<Claim>
        {
            new Claim("HumanVerified", "true"),
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // verification lifetime
            });

        // Redirect to protected page (change path if you like)
        return RedirectToPage("/Protected");
    }
}