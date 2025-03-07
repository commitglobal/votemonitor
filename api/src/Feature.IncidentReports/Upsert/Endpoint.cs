using System.Net;
using Feature.IncidentReports.Specifications;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.IncidentReports.Upsert;

public class Endpoint(
    IRepository<IncidentReport> repository,
    IReadRepository<PollingStationAggregate> pollingStationRepository,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    IReadRepository<FormAggregate> formRepository,
    IAuthorizationService authorizationService,
    ITimeProvider timeProvider) : Endpoint<Request, Results<Ok<IncidentReportModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-reports");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports", "mobile"));
        Summary(s => { s.Summary = "Upserts incident report"; });
    }

    public override async Task<Results<Ok<IncidentReportModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var formSpecification = new GetFormSpecification(req.ElectionRoundId, req.FormId);
        var form = await formRepository.FirstOrDefaultAsync(formSpecification, ct);
        if (form is null)
        {
            return TypedResults.NotFound();
        }

        if (form.Status == FormStatus.Drafted)
        {
            ThrowError(x => x.FormId, "Form is drafted");
        }

        if (form.FormType != FormType.IncidentReporting)
        {
            ThrowError(x => x.FormId, "Form is not of correct type");
        }

        var specification =
            new GetIncidentReportSpecification(req.ElectionRoundId, req.ObserverId, req.FormId, req.Id);
        var incidentReport = await repository.FirstOrDefaultAsync(specification, ct);

        List<BaseAnswer>? answers = null;
        if (req.Answers != null)
        {
            answers = req.Answers.Select(AnswerMapper.ToEntity).ToList();

            ValidateAnswers(answers, form);
        }

        return incidentReport is null
            ? await AddIncidentReportAsync(req, form, answers, ct)
            : await UpdateIncidentReportAsync(form, incidentReport, answers, req.IsCompleted,
                req.LastUpdatedAt ?? timeProvider.UtcNow, ct);
    }

    private async Task<Results<Ok<IncidentReportModel>, NotFound>> UpdateIncidentReportAsync(
        FormAggregate form,
        IncidentReport incidentReport,
        List<BaseAnswer>? answers,
        bool? isCompleted,
        DateTime? lastUpdatedAt,
        CancellationToken ct)
    {
        incidentReport = form.FillIn(incidentReport, answers, isCompleted, lastUpdatedAt ?? timeProvider.UtcNow);
        await repository.UpdateAsync(incidentReport, ct);

        return TypedResults.Ok(IncidentReportModel.FromEntity(incidentReport));
    }

    private async Task<Results<Ok<IncidentReportModel>, NotFound>> AddIncidentReportAsync(Request req,
        FormAggregate form,
        List<BaseAnswer>? answers,
        CancellationToken ct)
    {
        if (req.LocationType == IncidentReportLocationType.PollingStation)
        {
            var pollingStationSpecification =
                new GetPollingStationSpecification(req.ElectionRoundId, req.PollingStationId!.Value);
            var pollingStationExists = await pollingStationRepository.AnyAsync(pollingStationSpecification, ct);
            if (!pollingStationExists)
            {
                ThrowError(x => x.PollingStationId, "Polling station not found", (int)HttpStatusCode.NotFound);
            }
        }


        var monitoringObserverSpecification =
            new GetMonitoringObserverSpecification(req.ElectionRoundId, req.ObserverId);
        var monitoringObserver =
            await monitoringObserverRepository.FirstOrDefaultAsync(monitoringObserverSpecification, ct);
        if (monitoringObserver is null)
        {
            return TypedResults.NotFound();
        }

        var incidentReport = form.CreateIncidentReport(req.Id, monitoringObserver, req.LocationType,
            req.LocationDescription, req.PollingStationId, answers, req.IsCompleted,
            req.LastUpdatedAt ?? timeProvider.UtcNow);
        await repository.AddAsync(incidentReport, ct);

        return TypedResults.Ok(IncidentReportModel.FromEntity(incidentReport));
    }

    private void ValidateAnswers(List<BaseAnswer> answers, FormAggregate form)
    {
        var validationResult = AnswersValidator.GetValidationResults(answers, form.Questions);
        if (!validationResult.IsValid)
        {
            ValidationFailures.AddRange(validationResult.Errors);
            ThrowIfAnyErrors();
        }
    }
}
