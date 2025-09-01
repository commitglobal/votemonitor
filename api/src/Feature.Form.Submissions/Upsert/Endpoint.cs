using Module.Answers.Mappers;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.Form.Submissions.Upsert;

public class Endpoint(
    IRepository<FormSubmission> repository,
    IReadRepository<PollingStationAggregate> pollingStationRepository,
    IReadRepository<MonitoringObserver> monitoringObserverRepository,
    IReadRepository<Coalition> coalitionRepository,
    IReadRepository<FormAggregate> formRepository,
    IAuthorizationService authorizationService,
    ITimeProvider timeProvider) : Endpoint<Request, Results<Ok<FormSubmissionModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-submissions");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s =>
        {
            s.Summary = "Upserts form submission for a given polling station";
            s.Description = "When updating a submission it will update only the properties that are not null";
        });

        Policies(PolicyNames.ObserversOnly);
    }

    public override async Task<Results<Ok<FormSubmissionModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var coalitionFormSpecification =
            new GetCoalitionFormSpecification(req.ElectionRoundId, req.ObserverId, req.FormId);
        var ngoFormSpecification =
            new GetMonitoringNgoFormSpecification(req.ElectionRoundId, req.ObserverId, req.FormId);

        var form = (await coalitionRepository.FirstOrDefaultAsync(coalitionFormSpecification, ct)) ??
                   (await formRepository.FirstOrDefaultAsync(ngoFormSpecification, ct));
        if (form is null)
        {
            return TypedResults.NotFound();
        }

        if (form.Status == FormStatus.Drafted)
        {
            AddError(x => x.FormId, "Form is drafted");
            ThrowIfAnyErrors();
        }

        var specification =
            new GetFormSubmissionSpecification(req.ElectionRoundId, req.PollingStationId, req.FormId, req.ObserverId);
        var formSubmission = await repository.FirstOrDefaultAsync(specification, ct);

        List<BaseAnswer>? answers = null;
        if (req.Answers != null)
        {
            answers = req.Answers.Select(AnswerMapper.ToEntity).ToList();

            ValidateAnswers(answers, form);
        }

        return formSubmission is null
            ? await AddFormSubmissionAsync(req, form, answers, ct)
            : await UpdateFormSubmissionAsync(form, formSubmission, answers, req.IsCompleted, req.LastUpdatedAt, ct);
    }

    private async Task<Results<Ok<FormSubmissionModel>, NotFound>> UpdateFormSubmissionAsync(FormAggregate form,
        FormSubmission submission,
        List<BaseAnswer>? answers,
        bool? isCompleted,
        DateTime? lastUpdatedAt,
        CancellationToken ct)
    {
        submission = form.FillIn(submission, answers, isCompleted, lastUpdatedAt ?? timeProvider.UtcNow);
        await repository.UpdateAsync(submission, ct);

        return TypedResults.Ok(FormSubmissionModel.FromEntity(submission));
    }

    private async Task<Results<Ok<FormSubmissionModel>, NotFound>> AddFormSubmissionAsync(Request req,
        FormAggregate form,
        List<BaseAnswer>? answers,
        CancellationToken ct)
    {
        var pollingStationSpecification = new GetPollingStationSpecification(req.ElectionRoundId, req.PollingStationId);
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

        var submission = form.CreateFormSubmission(pollingStation, monitoringObserver, answers, req.IsCompleted,
            timeProvider.UtcNow, req.LastUpdatedAt ?? timeProvider.UtcNow);
        await repository.AddAsync(submission, ct);

        return TypedResults.Ok(FormSubmissionModel.FromEntity(submission));
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
