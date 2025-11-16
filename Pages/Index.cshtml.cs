using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Turnstile.Pages;

public class IndexModel : PageModel
{
    private readonly TurnstileValidator _turnstile;
    private readonly TurnstileOptions _options;

    // constructor
    public IndexModel(TurnstileValidator turnstile, IOptions<TurnstileOptions> options)
    {
        _turnstile = turnstile;
        _options   = options.Value;
    }

    // used by the .cshtml page to render the widget
    public string TurnstileSiteKey => _options.SiteKey;

    public void OnGet()
    {
        // normal GET handler (can be empty)
    }
}