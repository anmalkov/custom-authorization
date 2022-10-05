using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Shared.Extensions;
using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    // Declare the needed authorization policies here
    // More info: https://learn.microsoft.com/aspnet/core/security/authorization/policies
    options.AddPolicy("RequireAccessToSecret", policy => policy.RequireRole("AccessToSecret"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add your implementation of IAuthorizationService to the container instead of DummyAuthorizationService
builder.Services.AddScoped<IAuthorizationService, DummyAuthorizationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
// Use the UseInjectedRoles middleware to inject roles for the current user into the HttpContext
app.UseInjectedRoles();
app.UseAuthorization();

app.MapControllers();

app.Run();
