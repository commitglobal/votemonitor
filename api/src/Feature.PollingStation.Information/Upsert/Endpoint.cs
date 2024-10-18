using Authorization.Policies;
using Authorization.Policies.Requirements;
using Feature.PollingStation.Information.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Feature.PollingStation.Information.Upsert;

public class Endpoint(
    IRepository<PollingStationInformation> repository,
    IReadRepository<PollingStationAggregate> pollingStationRepository,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    IReadRepository<PollingStationInformationForm> formRepository,
    VoteMonitorContext context,
    IAuthorizationService authorizationService)
    : Endpoint<Request, Results<Ok<PollingStationInformationModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/information");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information", "mobile"));
        Summary(s => { s.Summary = "Upserts polling station information for a polling station"; });
        Policies(PolicyNames.ObserversOnly);
        RequestBinder(new RequestModelBinder());
    }

    public override async Task<Results<Ok<PollingStationInformationModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var formSpecification = new GetPollingStationInformationFormSpecification(req.ElectionRoundId);
        var form = await formRepository.FirstOrDefaultAsync(formSpecification, ct);
        if (form is null)
        {
            return TypedResults.NotFound();
        }

        var specification =
            new GetPollingStationInformationSpecification(req.ElectionRoundId, req.PollingStationId, req.ObserverId);
        var pollingStationInformation = await repository.FirstOrDefaultAsync(specification, ct);

        List<BaseAnswer>? answers = null;
        if (req.Answers != null)
        {
            answers = req.Answers.Select(AnswerMapper.ToEntity).ToList();

            ValidateAnswers(answers, form);
        }

        var observationBreaks = req.Breaks?.Select(x => ObservationBreak.Create(x.Start, x.End)).ToList();

        if (pollingStationInformation is null)
        {
            var pollingStationSpecification =
                new GetPollingStationSpecification(req.ElectionRoundId, req.PollingStationId);
            var pollingStation = await pollingStationRepository.FirstOrDefaultAsync(pollingStationSpecification, ct);
            if (pollingStation is null)
            {
                return TypedResults.NotFound();
            }

            var monitoringObserverSpecification =
                new GetMonitoringObserverSpecification(req.ElectionRoundId, req.ObserverId);
            var monitoringObserver =
                await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);
            if (monitoringObserver is null)
            {
                return TypedResults.NotFound();
            }

            pollingStationInformation = form.CreatePollingStationInformation(pollingStation,
                monitoringObserver,
                req.ArrivalTime,
                req.DepartureTime,
                answers,
                observationBreaks,
                req.IsCompleted);
        }
        else
        {
            pollingStationInformation = form.FillIn(pollingStationInformation,
                answers,
                req.ArrivalTime,
                req.DepartureTime,
                observationBreaks,
                req.IsCompleted);
        }

        await context
            .PollingStationInformation
            .Upsert(pollingStationInformation)
            .On(x => new
            {
                x.ElectionRoundId,
                x.PollingStationId,
                x.MonitoringObserverId,
                x.PollingStationInformationFormId
            })
            .AllowIdentityMatch()
            .WhenMatched((existing, inserting) => new PollingStationInformation
            {
                Id = existing.Id,
                IsCompleted = inserting.IsCompleted,
                ArrivalTime = inserting.ArrivalTime,
                DepartureTime = inserting.DepartureTime,
                Answers = inserting.Answers,
                Breaks = inserting.Breaks
            })
            .RunAsync(ct);

        return TypedResults.Ok(PollingStationInformationModel.FromEntity(pollingStationInformation));
    }

    private void ValidateAnswers(List<BaseAnswer> answers, PollingStationInformationForm form)
    {
        var validationResult = AnswersValidator.GetValidationResults(answers, form.Questions);
        if (!validationResult.IsValid)
        {
            ValidationFailures.AddRange(validationResult.Errors);
            ThrowIfAnyErrors();
        }
    }
}