using Microsoft.AspNetCore.Http;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Shared.Middlewares;

public class InjectRolesMiddleware
{
    private readonly RequestDelegate _next;

    public InjectRolesMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthorizationService authorizationService)
    {
        if (context.User.Identity is ClaimsIdentity claimsIdentity && claimsIdentity.IsAuthenticated)
        {
            foreach (var roleName in await authorizationService.GetRolesAsync(context))
            {
                claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, roleName));
            }
        }
        await _next(context);
    }
}
