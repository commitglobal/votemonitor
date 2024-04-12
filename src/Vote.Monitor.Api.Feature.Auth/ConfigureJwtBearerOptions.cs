using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Vote.Monitor.Api.Feature.Auth.Options;
using Vote.Monitor.Core.Exceptions;

namespace Vote.Monitor.Api.Feature.Auth;

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
            OnChallenge = context =>
            {
                context.HandleResponse();
                if (!context.Response.HasStarted)
                {
                    throw new UnauthorizedException("Authentication Failed.");
                }

                return Task.CompletedTask;
            },
            OnForbidden = _ => throw new ForbiddenException("You are not authorized to access this resource.")
        };
    }
}
