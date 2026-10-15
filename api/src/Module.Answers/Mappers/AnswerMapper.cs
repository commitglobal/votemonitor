using Module.Answers.Models;
using Module.Answers.Requests;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Module.Answers.Mappers;

public static class AnswerMapper
{
    public static BaseAnswerModel ToModel(BaseAnswer answer)
    {
        var model = MapToModel(answer);
        model.CreatedAt = answer.CreatedAt;
        model.LastUpdatedAt = answer.LastUpdatedAt;

        return model;
    }

    private static BaseAnswerModel MapToModel(BaseAnswer question)
    {
        switch (question)
        {
            case TextAnswer textInputAnswer:
                return TextAnswerModel.FromEntity(textInputAnswer);

            case NumberAnswer numberInputAnswerRequest:
                return NumberAnswerModel.FromEntity(numberInputAnswerRequest);

            case DateAnswer dateInputAnswerRequest:
                return DateAnswerModel.FromEntity(dateInputAnswerRequest);

            case TimeAnswer timeInputAnswerRequest:
                return TimeAnswerModel.FromEntity(timeInputAnswerRequest);

            case DateTimeAnswer dateTimeInputAnswerRequest:
                return DateTimeAnswerModel.FromEntity(dateTimeInputAnswerRequest);

            case SingleSelectAnswer singleSelectAnswerRequest:
                return SingleSelectAnswerModel.FromEntity(singleSelectAnswerRequest);

            case MultiSelectAnswer multiSelectAnswerRequest:
                return MultiSelectAnswerModel.FromEntity(multiSelectAnswerRequest);

            case RatingAnswer ratingAnswerRequest:
                return RatingAnswerModel.FromEntity(ratingAnswerRequest);

            default: throw new ApplicationException("Unknown question type");
        }
    }

    public static BaseAnswer ToEntity(BaseAnswerRequest answer)
    {
        return MapToEntity(answer).WithTimestamps(answer.CreatedAt, answer.LastUpdatedAt);
    }

    private static BaseAnswer MapToEntity(BaseAnswerRequest answer)
    {
        switch (answer)
        {
            case TextAnswerRequest textAnswer:
                return TextAnswer.Create(textAnswer.QuestionId, textAnswer.Text);

            case NumberAnswerRequest numberAnswer:
                return NumberAnswer.Create(numberAnswer.QuestionId, numberAnswer.Value);

            case DateAnswerRequest dateAnswer:
                return DateAnswer.Create(dateAnswer.QuestionId, dateAnswer.Date);

            case TimeAnswerRequest timeAnswer:
                return TimeAnswer.Create(timeAnswer.QuestionId, timeAnswer.Time);

            case DateTimeAnswerRequest dateTimeAnswer:
                return DateTimeAnswer.Create(dateTimeAnswer.QuestionId, dateTimeAnswer.DateTime);

            case SingleSelectAnswerRequest singleSelectAnswer:
                var selectedOption = ToEntity(singleSelectAnswer.Selection);

                return SingleSelectAnswer.Create(singleSelectAnswer.QuestionId, selectedOption);

            case MultiSelectAnswerRequest multiSelectAnswer:
                var selection = ToEntities(multiSelectAnswer.Selection);

                var multiSelectAnswerEntity = MultiSelectAnswer.Create(multiSelectAnswer.QuestionId, selection);

                return multiSelectAnswerEntity;

            case RatingAnswerRequest ratingAnswer:
                return RatingAnswer.Create(ratingAnswer.QuestionId, ratingAnswer.Value);

            default: throw new ApplicationException("Unknown question type");
        }
    }

    private static IReadOnlyList<SelectedOption> ToEntities(IEnumerable<SelectedOptionRequest>? options)
    {
        if (options is not null)
        {
            return options
                .Select(x=> ToEntity(x)!)
                .ToList()
                .AsReadOnly();
        }

        return Array.Empty<SelectedOption>();
    }

    private static SelectedOption? ToEntity(SelectedOptionRequest? option)
    {
        if(option is not null)
        {
            return SelectedOption.Create(option.OptionId, option.Text);
        }

        return null;
    }
}
