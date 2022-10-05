using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public interface IAuthorizationService
{
    Task<IEnumerable<string>> GetRolesAsync(HttpContext context);
}
