using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.Mappers;

public static class AnswerMapper
{
    public static BaseAnswerModel ToModel(BaseAnswer question)
    {
        switch (question)
        {
            case TextAnswer textInputAnswer:
                return TextAnswerModel.FromEntity(textInputAnswer);

            case NumberAnswer numberInputAnswerRequest:
                return NumberAnswerModel.FromEntity(numberInputAnswerRequest);

            case DateAnswer dateInputAnswerRequest:
                return DateAnswerModel.FromEntity(dateInputAnswerRequest);

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
        switch (answer)
        {
            case TextAnswerRequest textAnswer:
                return TextAnswer.Create(textAnswer.QuestionId, textAnswer.Text);

            case NumberAnswerRequest numberAnswer:
                return NumberAnswer.Create(numberAnswer.QuestionId, numberAnswer.Value);

            case DateAnswerRequest dateAnswer:
                return DateAnswer.Create(dateAnswer.QuestionId, dateAnswer.Date);

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

    private static IReadOnlyList<SelectedOption> ToEntities(IEnumerable<SelectedOptionRequest> options)
    {
        return options
            .Select(ToEntity)
            .ToList()
            .AsReadOnly();
    }

    private static SelectedOption ToEntity(SelectedOptionRequest option)
    {
        return SelectedOption.Create(option.OptionId, option.Text);
    }
}
