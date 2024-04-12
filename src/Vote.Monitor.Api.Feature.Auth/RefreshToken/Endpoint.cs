using Vote.Monitor.Api.Feature.Auth.Services;
using TokenResponse = Vote.Monitor.Api.Feature.Auth.Services.TokenResponse;

namespace Vote.Monitor.Api.Feature.Auth.RefreshToken;

public class Endpoint(ITokenService tokenService) : Endpoint<Request, Results<Ok<TokenResponse>, UnauthorizedHttpResult>>
{

    public override void Configure()
    {
        Post("/api/auth/refresh");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
    {
        return await tokenService.RefreshTokenAsync(req.Token, req.RefreshToken, ct);
    }
}
