using Microsoft.AspNetCore.Http;

namespace Shared.Services;

public class DummyAuthorizationService : IAuthorizationService
{
    public async Task<IEnumerable<string>> GetRolesAsync(HttpContext context)
    {
        // Place here your business logic to get roles for current HttpContext
        return await Task.FromResult(new[] { "AccessToSecret" });
    }
}
