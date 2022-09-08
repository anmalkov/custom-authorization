using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace Web.Razor.Pages
{
    public class SecretModel : PageModel
    {
        private readonly ILogger<SecretModel> _logger;

        public SecretModel(ILogger<SecretModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
