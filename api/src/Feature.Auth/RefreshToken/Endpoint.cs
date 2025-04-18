using Feature.Auth.Services;
using Services_TokenResponse = Feature.Auth.Services.TokenResponse;
using TokenResponse = Feature.Auth.Services.TokenResponse;

namespace Feature.Auth.RefreshToken;

public class Endpoint(ITokenService tokenService) : Endpoint<Request, Results<Ok<Services_TokenResponse>, UnauthorizedHttpResult>>
{

    public override void Configure()
    {
        Post("/api/auth/refresh");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<Services_TokenResponse>, UnauthorizedHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
    {
        return await tokenService.RefreshTokenAsync(req.Token, req.RefreshToken, ct);
    }
}
