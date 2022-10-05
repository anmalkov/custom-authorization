using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Shared.Extensions;
using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    // Declare the needed authorization policies here
    // More info: https://learn.microsoft.com/aspnet/core/security/authorization/policies
    options.AddPolicy("RequireAccessToSecret", policy => policy.RequireRole("AccessToSecret"));
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages(options => {
    options.Conventions.AllowAnonymousToPage("/Index");
    // This page is only accessible if all the requirements of "RequireAccessToSecret" policy are met
    // Apply your required policies here
    options.Conventions.AuthorizePage("/Secret", "RequireAccessToSecret");
}).AddMicrosoftIdentityUI();

// Add your implementation of IAuthorizationService to the container instead of DummyAuthorizationService
builder.Services.AddScoped<Shared.Services.IAuthorizationService, DummyAuthorizationService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
// Use the UseInjectedRoles middleware to inject roles for the current user into the HttpContext
app.UseInjectedRoles();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
