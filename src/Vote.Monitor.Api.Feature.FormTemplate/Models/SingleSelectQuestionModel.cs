using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Models;

public class SingleSelectQuestionModel : BaseQuestionModel
{
    public Guid Id { get; init; }
    public string Code { get; init; }
    public List<SelectOptionModel> Options { get; init; }

    public static SingleSelectQuestionModel FromEntity(SingleSelectQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            Helptext = question.Helptext,
            Options = question.Options.Select(SelectOptionModel.FromEntity).ToList()
        };
}
