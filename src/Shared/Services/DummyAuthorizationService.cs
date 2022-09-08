using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public class DummyAuthorizationService : IAuthorizationService
{
    public async Task<string[]> GetRolesAsync(HttpContext context)
    {
        // Place here your business logic to get roles for current HttpContext
        return await Task.FromResult(new[] { "AccessToSecret" });
    }
}
