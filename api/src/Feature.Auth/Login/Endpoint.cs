using Feature.Auth.Services;
using Services_TokenResponse = Feature.Auth.Services.TokenResponse;
using TokenResponse = Feature.Auth.Services.TokenResponse;

namespace Feature.Auth.Login;

public class Endpoint(ITokenService tokenService) : Endpoint<Request, Results<Ok<Services_TokenResponse>, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<Services_TokenResponse>, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        return await tokenService.GetTokenAsync(req.Email, req.Password, ct);
    }
}
