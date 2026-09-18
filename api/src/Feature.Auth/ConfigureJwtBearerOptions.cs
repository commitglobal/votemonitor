using System.Security.Claims;
using System.Text;
using Feature.Auth.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Feature.Auth;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JWTConfig _jwtConfig;

    public ConfigureJwtBearerOptions(IOptions<JWTConfig> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value;
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(string.Empty, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        byte[] key = Encoding.ASCII.GetBytes(_jwtConfig.TokenSigningKey);

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = async context =>
            {
                var userIdClaim = context.Principal?.FindFirst(ApplicationClaimTypes.UserId)?.Value;
                if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                {
                    context.Fail("Invalid user");
                    return;
                }

                var userManager = context.HttpContext.RequestServices
                    .GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.FindByIdAsync(userId.ToString());
                if (user is null || user.Status != UserStatus.Active)
                {
                    context.Fail("User is not active");
                }
            }
        };
    }
}
