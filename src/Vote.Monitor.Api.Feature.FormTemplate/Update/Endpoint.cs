using Vote.Monitor.Api.Feature.FormTemplate.Specifications;
using Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update;

public class Endpoint(IRepository<FormTemplateAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/form-templates/{id}");
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.GetByIdAsync(req.Id, ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetFormTemplateSpecification(req.Id, req.Code, req.FormType);
        var duplicatedFormTemplate = await repository.AnyAsync(specification, ct);

        if (duplicatedFormTemplate)
        {
            AddError(r => r.Name, "A form template with same parameters already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var sections = req.Sections.Select(section =>
         {
             var questions = section.Questions
                 .Select(MapQuestions)
                 .ToList()
                 .AsReadOnly();

             return FormSection.Create(section.Code, section.Title, questions);
         })
         .ToList()
         .AsReadOnly();

        formTemplate.UpdateDetails(req.Code, req.Name, req.FormType, req.Languages, sections);

        await repository.UpdateAsync(formTemplate, ct);
        return TypedResults.NoContent();
    }

    private static BaseQuestion MapQuestions(BaseQuestionRequest question)
    {
        switch (question)
        {
            case TextInputQuestionRequest textInputQuestion:
                return TextInputQuestion.Create(textInputQuestion.Id,
                    textInputQuestion.Code,
                    textInputQuestion.Text,
                    textInputQuestion.Helptext,
                    textInputQuestion.InputPlaceholder);

            case NumberInputQuestionRequest numberInputQuestion:
                return NumberInputQuestion.Create(numberInputQuestion.Id,
                     numberInputQuestion.Code,
                     numberInputQuestion.Text,
                     numberInputQuestion.Helptext,
                     numberInputQuestion.InputPlaceholder);

            case DateInputQuestionRequest dateInputQuestion:
                return DateInputQuestion.Create(dateInputQuestion.Id,
                    dateInputQuestion.Code,
                    dateInputQuestion.Text,
                    dateInputQuestion.Helptext);

            case SingleSelectQuestionRequest singleSelectQuestion:
                var singleSelectQuestionOptions = MapOptions(singleSelectQuestion.Options);

                return SingleSelectQuestion.Create(singleSelectQuestion.Id,
                    singleSelectQuestion.Code,
                    singleSelectQuestion.Text,
                    singleSelectQuestion.Helptext,
                    singleSelectQuestionOptions);

            case MultiSelectQuestionRequest multiSelectQuestion:
                var multiSelectQuestionOptions = MapOptions(multiSelectQuestion.Options);

                var multiSelectQuestionEntity = MultiSelectQuestion.Create(multiSelectQuestion.Id,
                    multiSelectQuestion.Code,
                    multiSelectQuestion.Text,
                    multiSelectQuestion.Helptext,
                    multiSelectQuestionOptions);

                return multiSelectQuestionEntity;

            case RatingQuestionRequest ratingQuestion:
                return RatingQuestion.Create(ratingQuestion.Id,
                     ratingQuestion.Code,
                     ratingQuestion.Text,
                     ratingQuestion.Helptext,
                     ratingQuestion.Scale);

            default: throw new ApplicationException("Unknown question type received");
        }
    }

    private static IReadOnlyList<SelectOption> MapOptions(IEnumerable<SelectOptionRequest> options)
    {
        return options
            .Select(o => SelectOption.Create(o.Id, o.Text, o.IsFreeText, o.IsFlagged))
            .ToList()
            .AsReadOnly();
    }
}
