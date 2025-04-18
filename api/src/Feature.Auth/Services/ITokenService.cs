namespace Feature.Auth.Services;

public interface ITokenService
{
    Task<Results<Ok<TokenResponse>, ValidationProblem>> GetTokenAsync(string email, string password, CancellationToken cancellationToken);
    Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> RefreshTokenAsync(string token, string refreshToken,
        CancellationToken ct);
}
