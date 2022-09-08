using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Web.Mvc.Models;

namespace Web.Mvc.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity && claimsIdentity.IsAuthenticated)
        {
            ViewData["roles"] = string.Join(", ", claimsIdentity.Claims.Where(c => c.Type == claimsIdentity.RoleClaimType).Select(c => c.Value));
        }
        return View();
    }

    [Authorize(Policy = "RequireAccessToSecret")]
    public IActionResult Secret()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
