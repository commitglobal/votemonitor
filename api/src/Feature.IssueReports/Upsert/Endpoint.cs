using System.Net;
using Authorization.Policies.Requirements;
using Feature.IssueReports.Models;
using Feature.IssueReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.IssueReports.Upsert;

public class Endpoint(
    IRepository<IssueReportAggregate> repository,
    IReadRepository<PollingStationAggregate> pollingStationRepository,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    IReadRepository<FormAggregate> formRepository,
    IAuthorizationService authorizationService) : Endpoint<Request, Results<Ok<IssueReportModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/issue-reports");
        DontAutoTag();
        Options(x => x.WithTags("issue-reports", "mobile"));
        Summary(s => { s.Summary = "Upserts issue report"; });
    }

    public override async Task<Results<Ok<IssueReportModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
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

        if (form.FormType == FormType.IssueReporting)
        {
            ThrowError(x => x.FormId, "Form is not of correct type");
        }

        var specification =
            new GetIssueReportSpecification(req.ElectionRoundId, req.ObserverId, req.FormId, req.IssueReportId);
        var issueReport = await repository.FirstOrDefaultAsync(specification, ct);

        List<BaseAnswer>? answers = null;
        if (req.Answers != null)
        {
            answers = req.Answers.Select(AnswerMapper.ToEntity).ToList();

            ValidateAnswers(answers, form);
        }

        return issueReport is null
            ? await AddIssueReportAsync(req, form, answers, ct)
            : await UpdateIssueReportAsync(form, issueReport, answers, ct);
    }

    private async Task<Results<Ok<IssueReportModel>, NotFound>> UpdateIssueReportAsync(
        FormAggregate form,
        IssueReportAggregate issueReport,
        List<BaseAnswer>? answers,
        CancellationToken ct)
    {
        issueReport = form.FillIn(issueReport, answers);
        await repository.UpdateAsync(issueReport, ct);

        return TypedResults.Ok(IssueReportModel.FromEntity(issueReport));
    }

    private async Task<Results<Ok<IssueReportModel>, NotFound>> AddIssueReportAsync(Request req,
        FormAggregate form,
        List<BaseAnswer>? answers,
        CancellationToken ct)
    {
        if (req.LocationType == IssueReportLocationType.PollingStation)
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

        var issueReport = form.CreateIssueReport(req.IssueReportId, monitoringObserver, req.LocationType,
            req.LocationDescription, req.PollingStationId, answers);
        await repository.AddAsync(issueReport, ct);

        return TypedResults.Ok(IssueReportModel.FromEntity(issueReport));
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