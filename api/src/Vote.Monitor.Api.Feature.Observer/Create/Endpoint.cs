﻿using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Api.Feature.Observer.Create;

public class Endpoint(UserManager<ApplicationUser> userManager,
    IRepository<ObserverAggregate> repository)
    : Endpoint<Request, Results<Ok<ObserverModel>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/observers");
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(x => { x.Description = "Creates account for an observer"; });
    }

    public override async Task<Results<Ok<ObserverModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);

        if (user is not null)
        {
            AddError(r => r.Email, "A user with same email already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var applicationUser = ApplicationUser.CreateObserver(req.FirstName, req.LastName, req.Email, req.PhoneNumber, req.Password);

        var result = await userManager.CreateAsync(applicationUser);
        if (!result.Succeeded)
        {
            AddError(r => r.Email, result.GetAllErrors());
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var observer = ObserverAggregate.Create(applicationUser);
        await repository.AddAsync(observer, ct);

        return TypedResults.Ok(new ObserverModel
        {
            Id = observer.Id,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            Email = applicationUser.Email!,
            PhoneNumber = applicationUser.PhoneNumber!,
            Status = applicationUser.Status,
            CreatedOn = observer.CreatedOn,
            LastModifiedOn = observer.LastModifiedOn
        });
    }
}
