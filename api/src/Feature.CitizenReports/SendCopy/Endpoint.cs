using Feature.CitizenReports.Specifications;
using Job.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Options;
using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.EmailTemplating.Props;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Feature.CitizenReports.SendCopy;

public class Endpoint(
    IReadRepository<CitizenReportAggregate> repository,
    IReadRepository<FormAggregate> formRepository,
    IJobService jobService,
    IEmailTemplateFactory emailFactory,
    IOptions<ApiConfiguration> options,
    ILogger<Endpoint> logger) : Endpoint<Request, NoContent>
{
    private readonly ApiConfiguration _apiConfiguration = options.Value;

    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}:sendCopy");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-reports", "public"));
        Summary(s => { s.Summary = "Sends a copy of citizen report to an given email"; });
    }

    public override async Task<NoContent> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var formSpecification = new GetFormSpecification(req.ElectionRoundId, req.FormId);
        var form = await formRepository.FirstOrDefaultAsync(formSpecification, ct);

        if (form == null)
        {
            logger.LogWarning("Could not find citizen reporting form {ElectionRoundId}, {FormId}", req.ElectionRoundId,
                req.FormId);
            return TypedResults.NoContent();
        }

        var citizenReportByIdSpecification =
            new GetCitizenReportByIdSpecification(req.ElectionRoundId, req.FormId, req.CitizenReportId);
        var citizenReport = await repository.FirstOrDefaultAsync(
            citizenReportByIdSpecification, ct);

        if (citizenReport == null)
        {
            logger.LogWarning("Could not find citizen report {ElectionRoundId}, {FormId}, {CitizenReportId}",
                req.ElectionRoundId, req.FormId, req.CitizenReportId);
            return TypedResults.NoContent();
        }

        var email = emailFactory.GenerateCitizenReportEmail(new CitizenReportEmailProps(
            $"Citizen report #{req.CitizenReportId}", $"Citizen report #{req.CitizenReportId}",
            $"Citizen report #{req.CitizenReportId}",
            MapAnswersToEmailFragmentProps(form, citizenReport.Answers), _apiConfiguration.WebAppUrl));

        jobService.EnqueueSendEmail(req.Email, email.Subject, email.Body);


        return TypedResults.NoContent();
    }

    private IEnumerable<BaseAnswerFragmentProps> MapAnswersToEmailFragmentProps(FormAggregate form,
        IReadOnlyList<BaseAnswer> citizenReportAnswers)
    {
        var result = new List<BaseAnswerFragmentProps>();

        foreach (var answer in citizenReportAnswers)
        {
            var question = form.Questions.FirstOrDefault(x => x.Id == answer.QuestionId);
            if (question == null)
            {
                logger.LogWarning("Unknown question id received {formId} {questionId}", form.Id, answer.QuestionId);
                continue;
            }

            switch (answer)
            {
                case DateAnswer dateAnswer:
                    result.Add(new InputAnswerFragmentProps(question.Text[form.DefaultLanguage],
                        dateAnswer.Date.ToString("yyyy-MM-dd HH:mm")));
                    break;

                case NumberAnswer numberAnswer:
                    result.Add(new InputAnswerFragmentProps(question.Text[form.DefaultLanguage],
                        numberAnswer.Value.ToString()));
                    break;

                case TextAnswer textAnswer:
                    result.Add(new InputAnswerFragmentProps(question.Text[form.DefaultLanguage], textAnswer.Text));
                    break;

                case RatingAnswer ratingAnswer:
                    var ratingQuestion = (question as RatingQuestion)!;
                    result.Add(new RatingAnswerFragmentProps(question.Text[form.DefaultLanguage],
                        ratingQuestion.Scale.UpperBound, ratingAnswer.Value));
                    break;

                case SingleSelectAnswer singleSelectAnswer:
                    var singleSelectQuestion = (question as SingleSelectQuestion)!;
                    result.Add(new SelectAnswerFragmentProps(question.Text[form.DefaultLanguage],
                        MapOptionsToEmailFragmentProps(form.DefaultLanguage, singleSelectQuestion.Options,
                            [singleSelectAnswer.Selection])));
                    break;

                case MultiSelectAnswer multiSelectAnswer:
                    var multiSelectQuestion = (question as MultiSelectQuestion)!;

                    result.Add(new SelectAnswerFragmentProps(question.Text[form.DefaultLanguage],
                        MapOptionsToEmailFragmentProps(form.DefaultLanguage, multiSelectQuestion.Options,
                            multiSelectAnswer.Selection)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(answer));
            }
        }

        return result;
    }

    private IEnumerable<SelectAnswerOptionFragmentProps> MapOptionsToEmailFragmentProps(string defaultLanguage,
        IEnumerable<SelectOption> options, IEnumerable<SelectedOption> selection)
    {
        var result = new List<SelectAnswerOptionFragmentProps>();
        var selectedOptions = selection as SelectedOption[] ?? selection.ToArray();

        foreach (var option in options)
        {
            var selectedOption = selectedOptions.FirstOrDefault(x => x.OptionId == option.Id);
            result.Add(new SelectAnswerOptionFragmentProps(option.Text[defaultLanguage] + (selectedOption?.Text ?? ""),
                selection != null));
        }

        return result;
    }
}