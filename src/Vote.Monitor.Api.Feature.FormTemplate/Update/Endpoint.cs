using System;
using Vote.Monitor.Api.Feature.FormTemplate.Specifications;
using Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;
using Vote.Monitor.Domain.Entities.LanguageAggregate;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update;

public class Endpoint(IRepository<FormTemplateAggregate> repository,
    IReadRepository<Language> languagesRepository) : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Put("/api/form-templates/{id}");
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formTemplate = await repository.FirstOrDefaultAsync(new GetByIdSpecification(req.Id), ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetFormTemplate(req.Id, req.Code, req.FormType);
        var duplicatedFormTemplate = await repository.AnyAsync(specification, ct);

        if (duplicatedFormTemplate)
        {
            AddError(r => r.Name, "A form template with same parameters already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var languages = await languagesRepository.ListAsync(new LanguagesByIsoCode(req.Languages), ct);
        formTemplate.UpdateDetails(req.Code, req.Name, req.FormType, languages);

        formTemplate.ClearSections();

        foreach (var section in req.Sections)
        {
            var formSection = formTemplate.AddFormSection(section.Code, section.Title);
            foreach (var question in section.Questions)
            {
                switch (question)
                {
                    case TextInputQuestionRequest textInputQuestion:
                        formSection.AddTextInputQuestion(textInputQuestion.Id,
                            textInputQuestion.Code,
                            textInputQuestion.Text,
                            textInputQuestion.Helptext,
                            textInputQuestion.InputPlaceholder);
                        break;

                    case NumberInputQuestionRequest numberInputQuestion:
                        formSection.AddNumberInputQuestion(numberInputQuestion.Id,
                            numberInputQuestion.Code,
                            numberInputQuestion.Text,
                            numberInputQuestion.Helptext,
                            numberInputQuestion.InputPlaceholder);
                        break;

                    case DateInputQuestionRequest dateInputQuestion:
                        formSection.AddDateInputQuestion(dateInputQuestion.Id, dateInputQuestion.Code, dateInputQuestion.Text,
                            dateInputQuestion.Helptext);
                        break;

                    case SingleSelectQuestionRequest singleSelectQuestion:
                        var singleSelectQuestionEntity = formSection.AddSingleSelectQuestion(singleSelectQuestion.Id,
                            singleSelectQuestion.Code,
                            singleSelectQuestion.Text,
                            singleSelectQuestion.Helptext);

                        foreach (var option in singleSelectQuestion.Options)
                        {
                            singleSelectQuestionEntity.AddOption(option.Id,
                                option.Text,
                                option.IsFreeText,
                                option.IsFlagged);
                        }
                        break;

                    case MultiSelectQuestionRequest multiSelectQuestion:
                        var multiSelectQuestionEntity = formSection.AddMultiSelectQuestion(multiSelectQuestion.Id,
                            multiSelectQuestion.Code,
                            multiSelectQuestion.Text,
                            multiSelectQuestion.Helptext);

                        foreach (var option in multiSelectQuestion.Options)
                        {
                            multiSelectQuestionEntity.AddOption(option.Id,
                                option.Text,
                                option.IsFlagged,
                                option.IsFreeText);
                        }
                        break;

                    case RatingQuestionRequest ratingQuestion:
                        formSection.AddRatingQuestion(ratingQuestion.Id,
                            ratingQuestion.Code,
                            ratingQuestion.Text,
                            ratingQuestion.Helptext,
                            ratingQuestion.Scale);
                        break;

                    case GridQuestionRequest gridQuestion:
                        var gridQuestionEntity = formSection.AddGridQuestion(
                            gridQuestion.Id,
                            gridQuestion.Text,
                            gridQuestion.Helptext,
                            gridQuestion.ScalePlaceholder,
                            gridQuestion.Scale,
                            gridQuestion.HasNotKnownColumn);

                        foreach (var row in gridQuestion.Rows)
                        {
                            gridQuestionEntity.AddRow(row.Id, row.Code, row.Text, row.Helptext);
                        }

                        break;

                    default: throw new ApplicationException("Unknown question type received");
                }
            }
        }

        await repository.UpdateAsync(formTemplate, ct);
        return TypedResults.NoContent();
    }
}
