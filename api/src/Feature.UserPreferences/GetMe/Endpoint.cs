using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Repository;

namespace Feature.UserPreferences.GetMe;

public class Endpoint(IReadRepository<ApplicationUser> repository)
    : Endpoint<Request, Results<Ok<UserModel>, NotFound<string>>>
{
    public override void Configure()
    {
        Get("/api/users/me");
    }

    public override async Task<Results<Ok<UserModel>, NotFound<string>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(req.Id, ct);
        if (user is null)
        {
            return TypedResults.NotFound("User not found");
        }

        return TypedResults.Ok(new UserModel
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}",
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            Status = user.Status,
            Preferences = user.Preferences
        });
    }
}
