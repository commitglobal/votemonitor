using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Feature.Auth.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.NgoAdminAggregate;

namespace Feature.Auth.Services;

internal class TokenService(UserManager<ApplicationUser> userManager,
    IReadRepository<NgoAdmin> ngoAdminRepository,
    IOptions<JWTConfig> jwtOptions,
    ITimeProvider timeProvider,
    ILogger<TokenService> logger) : ITokenService
{

    private readonly JWTConfig _jwtConfig = jwtOptions.Value;

    public async Task<Results<Ok<TokenResponse>, ValidationProblem>> GetTokenAsync(string email, string password, CancellationToken cancellationToken)
    {
        var validationCtx = ValidationContext.Instance;
        if (await userManager.FindByEmailAsync(email.Trim()) is not { }
                user
            || !await userManager.CheckPasswordAsync(user, password))
        {
            validationCtx.AddError("Invalid username or password");
            logger.LogWarning("Authentication failed for {email}", email);
            return TypedResults.ValidationProblem(validationCtx.ValidationFailures.ToValidationErrorDictionary());
        }

        return TypedResults.Ok(await GenerateTokensAndUpdateUser(user));
    }

    public async Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> RefreshTokenAsync(string token,
        string refreshToken, CancellationToken ct)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(token);
        if (userPrincipal is null)
        {
            return TypedResults.Unauthorized();
        }

        string? userEmail = userPrincipal.GetEmail();
        var user = await userManager.FindByEmailAsync(userEmail!.Trim());
        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= timeProvider.UtcNow)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(await GenerateTokensAndUpdateUser(user));
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(ApplicationUser user)
    {
        var claims = await GetClaims(user);
        string token = GenerateEncryptedToken(GetSigningCredentials(), claims);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiryTime = timeProvider.UtcNow.AddDays(_jwtConfig.RefreshTokenExpirationInDays);
        user.UpdateRefreshToken(refreshToken, refreshTokenExpiryTime);

        await userManager.UpdateAsync(user);
        return new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime, user.Role);
    }

    private async Task<IEnumerable<Claim>> GetClaims(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ApplicationClaimTypes.UserId, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Role, user.Role),
            new(ApplicationClaimTypes.Role, user.Role)
        };

        if (user.Role == UserRole.NgoAdmin)
        {
            var ngoAdmin = await ngoAdminRepository.GetByIdAsync(user.Id);
            claims.Add(new Claim(ApplicationClaimTypes.NgoId, ngoAdmin?.NgoId.ToString() ?? string.Empty));
        }
        return claims;
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: timeProvider.UtcNow.AddMinutes(_jwtConfig.TokenExpirationInMinutes),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.TokenSigningKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_jwtConfig.TokenSigningKey);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}
internal static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.Email);
}
