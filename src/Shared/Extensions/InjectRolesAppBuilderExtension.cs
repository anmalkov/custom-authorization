using Microsoft.AspNetCore.Builder;
using Shared.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extensions;

public static class InjectRolesAppBuilderExtension
{
    public static IApplicationBuilder UseInjectedRoles(this IApplicationBuilder app)
    {
        return app.UseMiddleware<InjectRolesMiddleware>();
    }
}
