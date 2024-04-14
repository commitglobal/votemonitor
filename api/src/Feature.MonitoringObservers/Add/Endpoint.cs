using Authorization.Policies;
using Feature.MonitoringObservers.Specifications;
using Job.Contracts;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.EmailTemplating.Props;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;
using Vote.Monitor.Domain.Entities.ObserverAggregate;

namespace Feature.MonitoringObservers.Add;

public class Endpoint(
    IJobService jobService,
    IEmailTemplateFactory emailFactory,
    IRepository<ElectionRoundAggregate> repository,
    IRepository<MonitoringNgoAggregate> monitoringNgoRepository,
    IReadRepository<Observer> observerRepository,
    IRepository<MonitoringObserver> monitoringObserverRepository)
    : Endpoint<Request, Results<Ok<Response>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers");
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Policies(PolicyNames.PlatformAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Permanently removes an monitoring observer from monitoring ngo";
            s.Description = "All data will be lost. Only PlatformAdmins can perform this operation";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound<string>, Conflict<ProblemDetails>, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound = await repository.GetByIdAsync(req.ElectionRoundId, ct);
        if (electionRound is null)
        {
            return TypedResults.NotFound("Election round not found");
        }

        var monitoringNgo = await monitoringNgoRepository.SingleOrDefaultAsync(new GetMonitoringNgoWithObserversSpecification(req.ElectionRoundId, req.MonitoringNgoId), ct);
        if (monitoringNgo is null)
        {
            return TypedResults.NotFound("Monitoring NGO not found");
        }

        if (monitoringNgo.Ngo.Status == NgoStatus.Deactivated || monitoringNgo.Status == MonitoringNgoStatus.Suspended)
        {
            AddError(x => x.MonitoringNgoId, "Only active monitoring NGOs can add monitoring observers");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var observer = await observerRepository.GetByIdAsync(req.ObserverId, ct);
        if (observer is null)
        {
            return TypedResults.NotFound("Observer not found");
        }

        if (observer.ApplicationUser.Status == UserStatus.Deactivated)
        {
            AddError(x => x.ObserverId, "Only active observers can monitor elections");
            return TypedResults.ValidationProblem(ValidationFailures.ToValidationErrorDictionary());
        }

        var monitoringObserver = monitoringNgo.AddMonitoringObserver(observer);
        if (monitoringObserver is null)
        {
            AddError(x => x.ObserverId, "Observer is already registered as monitoring for this monitoring NGO.");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }
     
        await monitoringObserverRepository.AddAsync(monitoringObserver, ct);

        var email = emailFactory.GenerateEmail(EmailTemplateType.InvitationExistingUser,
            new InvitationExistingUserEmailProps("", ""));
        jobService.SendEmail(observer.ApplicationUser.Email!, email.Subject, email.Body);

        return TypedResults.Ok(new Response
        {
            Id = monitoringObserver.Id,
            InviterNgoId = monitoringObserver.MonitoringNgoId,
            ObserverId = observer.Id
        });
    }
}
