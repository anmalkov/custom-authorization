using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Web.Razor.Pages;

public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    public string Roles { get; set; }

    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity && claimsIdentity.IsAuthenticated)
        {
            Roles = string.Join(", ", claimsIdentity.Claims.Where(c => c.Type == claimsIdentity.RoleClaimType).Select(c => c.Value));
        }
    }
}

