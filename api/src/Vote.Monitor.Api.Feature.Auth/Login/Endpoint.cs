using Vote.Monitor.Api.Feature.Auth.Services;
using TokenResponse = Vote.Monitor.Api.Feature.Auth.Services.TokenResponse;

namespace Vote.Monitor.Api.Feature.Auth.Login;

public class Endpoint(ITokenService tokenService) : Endpoint<Request, Results<Ok<TokenResponse>, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<TokenResponse>, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        return await tokenService.GetTokenAsync(req.Email, req.Password, ct);
    }
}
