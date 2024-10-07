using Feature.CitizenReports.Models;
using Feature.CitizenReports.Specifications;
using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Domain.Entities.FormAnswerBase;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.LocationAggregate;

namespace Feature.CitizenReports.Upsert;

public class Endpoint(
    IRepository<CitizenReportAggregate> repository,
    IReadRepository<Location> locationRepository,
    IReadRepository<FormAggregate> formRepository) : Endpoint<Request, Results<Ok<CitizenReportModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-reports", "public"));
        Summary(s => { s.Summary = "Upserts citizen report for a given form at a given location"; });
    }

    public override async Task<Results<Ok<CitizenReportModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var locationSpecification = new GetLocationSpecification(req.ElectionRoundId, req.LocationId);
        var location = await locationRepository.FirstOrDefaultAsync(locationSpecification, ct);
        if (location is null)
        {
            return TypedResults.NotFound();
        }

        var formSpecification = new GetFormSpecification(req.ElectionRoundId, req.FormId);
        var form = await formRepository.FirstOrDefaultAsync(formSpecification, ct);
        if (form is null)
        {
            return TypedResults.NotFound();
        }

        var specification =
            new GetCitizenReportByIdSpecification(req.ElectionRoundId, req.FormId, req.CitizenReportId);
        var citizenReport = await repository.FirstOrDefaultAsync(specification, ct);

        List<BaseAnswer>? answers = null;
        if (req.Answers != null)
        {
            answers = req.Answers.Select(AnswerMapper.ToEntity).ToList();

            ValidateAnswers(answers, form);
        }

        return citizenReport is null
            ? await AddFormSubmissionAsync(req, form, location, answers, ct)
            : await UpdateFormSubmissionAsync(form, citizenReport, answers, ct);
    }

    private async Task<Results<Ok<CitizenReportModel>, NotFound>> UpdateFormSubmissionAsync(
        FormAggregate form,
        CitizenReportAggregate citizenReport,
        List<BaseAnswer>? answers,
        CancellationToken ct)
    {
        citizenReport = form.FillIn(citizenReport, answers);
        await repository.UpdateAsync(citizenReport, ct);

        return TypedResults.Ok(CitizenReportModel.FromEntity(citizenReport));
    }

    private async Task<Results<Ok<CitizenReportModel>, NotFound>> AddFormSubmissionAsync(Request req,
        FormAggregate form,
        Location location,
        List<BaseAnswer>? answers,
        CancellationToken ct)
    {
        var submission = form.CreateCitizenReport(req.CitizenReportId, location, answers);
        await repository.AddAsync(submission, ct);

        return TypedResults.Ok(CitizenReportModel.FromEntity(submission));
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