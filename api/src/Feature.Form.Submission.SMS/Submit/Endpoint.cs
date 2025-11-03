using FastEndpoints;
using Feature.Form.Submission.SMS.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.Ingestor.Core.Converters;
using static Vote.Monitor.Ingestor.Core.Converters.SmsFormSubmission;
namespace Feature.Form.Submission.SMS.Submit;
public class Endpoint(
    IRepository<FormSubmission> repository,
    IReadRepository<FormAggregate> formRepository,
    IReadRepository<MonitoringObserverAggregate> monitoringObserverRepository,
    IReadRepository<PollingStationAggregate> pollingStationRepository,
    SmsToFormSubmissionDecoder formSubmissionDecoder,
    ITimeProvider timeProvider) : Endpoint<Request, Results<Ok<FormSubmissionModel>, NotFound>>
{
    private const int MAX_NUMBER_ANSWER_LENGTH = 4;

    public override void Configure()
    {
        Post("/api/election-rounds/sms-form-submissions");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Submits form submission for a given polling station using sms format";
            s.Description = "Decodes sms format of the form submission, validates it and saves it";
        });
    }

    public override async Task<Results<Ok<FormSubmissionModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        SmsFormSubmission decodedSubmission;

        try
        {
            decodedSubmission = formSubmissionDecoder.Decode(req.SmsMessage);
        }
        catch (ArgumentException ex)
        {
            AddError(x => x.SmsMessage, ex.Message);
            ThrowIfAnyErrors();
            return null;
        }

        var form = await formRepository.FirstOrDefaultAsync(
            new GetFormSpecification(decodedSubmission.FormCode, req.ElectionRoundId, req.MonitoringNgoId), ct);
        if (form is null)
        {
            return TypedResults.NotFound();
        }

        if (form.Status == FormStatus.Drafted)
        {
            AddError(x => x.SmsMessage, "Form is drafted");
            ThrowIfAnyErrors();
        }

        var monitoringObserver = await monitoringObserverRepository.FirstOrDefaultAsync(
            new GetMonitoringObserver(req.PhoneNumber, req.MonitoringNgoId, req.ElectionRoundId), ct);

        if (monitoringObserver is null)
        {
            return TypedResults.NotFound();
        }

        var pollingStation = await pollingStationRepository.FirstOrDefaultAsync(
            new GetPollingStation(decodedSubmission.PollingStationCode, req.ElectionRoundId), ct);

        if (pollingStation is null) {
            return TypedResults.NotFound();
        }

        var answers = MapAnswers(form, decodedSubmission);
        ThrowIfAnyErrors();

        var formSubmission = form.CreateFormSubmission(pollingStation, monitoringObserver, answers, isCompleted: true, timeProvider.UtcNow);

        await repository.AddAsync(formSubmission, ct);

        return TypedResults.Ok(FormSubmissionModel.FromEntity(formSubmission));
    }

    private List<BaseAnswer> MapAnswers(FormAggregate form, SmsFormSubmission smsFormSubmission)
    {
        var answerList = new Dictionary<string, BaseAnswer>();
        var questionMap = form.Questions.ToDictionary(q => q.Code);
        //TODO: make sure all questions are answered
        foreach (var answer in smsFormSubmission.Answers) {
            if (!questionMap.ContainsKey(answer.Code))
            {
                AddError(x => x.SmsMessage, $"Question '{answer.Code}' does not exist in form '{form.Code}'.");
                continue;
            }

            var question = questionMap[answer.Code];
            switch (question)
            {
                case NumberQuestion numberQuestion:
                    TryAddNumberAnswer(numberQuestion, answer, answerList);
                    break;
                case SingleSelectQuestion singleSelectQuestion:
                    TryAddSingleSelectAnswer(singleSelectQuestion, answer, answerList);
                    break;
                case MultiSelectQuestion multiSelectQuestion:
                    TryAddMultiSelectAnswer(multiSelectQuestion, answer, answerList);
                    break;
                default:
                    AddError(x => x.SmsMessage, $"Question {question.Code} is not of a supported type.");
                    break;
            }
        }

        if(answerList.Count != form.Questions.Count)
        {
            AddError(x => x.SmsMessage, $"The number of questions and answers does not match.");
        }



        return answerList.Values.ToList();
    }

    private void TryAddNumberAnswer(NumberQuestion question, SmsAnswer answer, Dictionary<string, BaseAnswer> answerList)
    {
        if (!int.TryParse(answer.Value, out var value))
        {
            AddError(x => x.SmsMessage, $"The answer {answer.Value} for question {question.Code} is not a valid number.");
            return;
        }

        if (answer.Value.Length > MAX_NUMBER_ANSWER_LENGTH)
        {
            AddError(x => x.SmsMessage, $"The answer {answer.Value} for question {question.Code} must have 4 digits or less.");
            return;
        }

        answerList[question.Code] = NumberAnswer.Create(question.Id, value);
    }

    private void TryAddSingleSelectAnswer(SingleSelectQuestion question, SmsAnswer answer, Dictionary<string, BaseAnswer> answerList)
    {
        var selectOption = GetSelectOption(question.Options, question.Code, answer.Value);

        if (selectOption == null)
        {
            return;
        }

        answerList[question.Code] = SingleSelectAnswer.Create(question.Id, SelectedOption.Create(selectOption.Id, string.Empty));
    }

    private void TryAddMultiSelectAnswer(MultiSelectQuestion question, SmsAnswer answer, Dictionary<string, BaseAnswer> answerList)
    {
        bool isInvalid = false;
        List<SelectedOption> selectedOptions = new List<SelectedOption>();
        foreach(char c in answer.Value)
        {
            var selectOption = GetSelectOption(question.Options, question.Code, c.ToString());
            if (selectOption == null)
            {
                isInvalid = true;
            } else {
                selectedOptions.Add(SelectedOption.Create(selectOption.Id, string.Empty));
            }
        }

        if (isInvalid)
        {
            return;
        }

        answerList[question.Code] = MultiSelectAnswer.Create(question.Id, selectedOptions.AsReadOnly());
    }

    private SelectOption? GetSelectOption(IReadOnlyList<SelectOption> options, string questionCode, string answer)
    {
        if (!int.TryParse(answer, out var value))
        {
            AddError(x => x.SmsMessage, $"The answer {answer} for question {questionCode} is not a valid option.");
            return null;
        }

        var index = value - 1;

        if (index < 0 || index >= options.Count)
        {
            AddError(x => x.SmsMessage, $"The answer {answer} for question {questionCode} is not a valid option.");
            return null;
        }
        return options[index];
    }
}
