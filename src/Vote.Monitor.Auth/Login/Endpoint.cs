using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Vote.Monitor.Auth.Options;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.Auth.Login;

public class Endpoint : Endpoint<Request, Results<Ok<Response>, ProblemDetails>>
{
    private readonly IReadRepository<ApplicationUser> _repository;
    private readonly JWTConfig _options;

    public Endpoint(IReadRepository<ApplicationUser> repository, IOptions<JWTConfig> options)
    {
        _repository = repository;
        _options = options.Value;
    }

    public override void Configure()
    {
        Post("/api/auth");
        DontAutoTag();
        Description(x => x.WithTags("Auth"));
        AllowAnonymous();
    }

    public override async Task<Results<Ok<Response>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetApplicationUserSpecification(req.Username, req.Password);
        var user = await _repository.FirstOrDefaultAsync(specification, ct);

        if (user is null)
        {
            AddError("Invalid username or password");
            return new ProblemDetails(ValidationFailures);
        }

        var jwtToken = JWTBearer.CreateToken(
            signingKey: _options.TokenSigningKey,
            expireAt: DateTime.UtcNow.AddDays(1),
            roles: new[] { user.Role.Name },
            claims: new (string claimType, string claimValue)[] { (JwtRegisteredClaimNames.Sub, user.Id.ToString()) }
        );

        return TypedResults.Ok(new Response() { Token = jwtToken });
    }
}
