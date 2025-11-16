using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

[Authorize(Policy = "HumanVerified")]
public class ProtectedModel : PageModel
{
    public void OnGet()
    {
        
    }
}