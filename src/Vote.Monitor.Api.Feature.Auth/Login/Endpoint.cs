using Vote.Monitor.Api.Feature.Auth.Options;
using Vote.Monitor.Api.Feature.Auth.Specifications;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Api.Feature.Auth.Login;

public class Endpoint : Endpoint<Request, Results<Ok<Response>, ProblemDetails>>
{
    private readonly IReadRepository<ApplicationUser> _repository;
    private readonly IReadRepository<NgoAdmin> _ngoAdminRepository;
    private readonly IReadRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly JWTConfig _options;

    public Endpoint(IReadRepository<ApplicationUser> repository,
        IReadRepository<NgoAdmin> ngoAdminRepository,
        IReadRepository<MonitoringObserver> monitoringObserverRepository,
        IOptions<JWTConfig> options)
    {
        _repository = repository;
        _ngoAdminRepository = ngoAdminRepository;
        _monitoringObserverRepository = monitoringObserverRepository;
        _options = options.Value;
    }

    public override void Configure()
    {
        Post("/api/auth");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<Response>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetApplicationUserSpecification(req.Username, req.Password);
        var user = await _repository.SingleOrDefaultAsync(specification, ct);

        if (user is null)
        {
            AddError("Invalid username or password");
            return new ProblemDetails(ValidationFailures);
        }

        var claims = new List<(string claimType, string claimValue)> { (JwtRegisteredClaimNames.Sub, user.Id.ToString()) };

        if (user.Role == UserRole.NgoAdmin)
        {
            var ngoAdminSpecification = new GetNgoAdminSpecification(user.Id);
            var ngoAdmin = await _ngoAdminRepository.FirstOrDefaultAsync(ngoAdminSpecification, ct);
            if (ngoAdmin is not null)
            {
                claims.Add((ApplicationClaimTypes.NgoId, ngoAdmin.NgoId.ToString()));
            }
        }

        var jwtToken = JWTBearer.CreateToken(
            signingKey: _options.TokenSigningKey,
            expireAt: DateTime.UtcNow.AddDays(1),
            roles: new[] { user.Role.Name },
            claims: claims.ToArray()
        );

        return TypedResults.Ok(new Response { Token = jwtToken });
    }
}
