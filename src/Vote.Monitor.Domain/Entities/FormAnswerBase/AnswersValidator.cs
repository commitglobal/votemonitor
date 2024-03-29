using FluentValidation.Results;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormAnswerBase;

public static class AnswersValidator
{
    public static ValidationResult GetValidationResults(List<BaseAnswer> answers, IReadOnlyList<BaseQuestion> questions)
    {
        var questionsById = questions.ToDictionary(x => x.Id);

        var questionExistence = ValidateQuestionExistence(answers, questionsById);
        return !questionExistence.IsValid ? questionExistence : ValidateAnswers(answers, questionsById);
    }

    private static ValidationResult ValidateQuestionExistence(List<BaseAnswer> answers, Dictionary<Guid, BaseQuestion> questionsById)
    {
        var validationResults = new List<ValidationResult>();
        for (int index = 0; index < answers.Count; index++)
        {
            var answer = answers[index];

            if (!questionsById.ContainsKey(answer.QuestionId))
            {
                validationResults.Add(GetUnknownQuestionError(index, answer));
            }
        }

        return new ValidationResult(validationResults);
    }

    private static ValidationResult ValidateAnswers(List<BaseAnswer> answers, Dictionary<Guid, BaseQuestion> questionsById)
    {
        var validationResults = new List<ValidationResult>();
        for (int index = 0; index < answers.Count; index++)
        {
            var answer = answers[index];
            var question = questionsById[answer.QuestionId];
            validationResults.Add(answer.Validate(question, index));
        }

        return new ValidationResult(validationResults);
    }

    private static ValidationResult GetUnknownQuestionError(int answerIndex, BaseAnswer answer)
    {
        var propertyName = $"answers[{answerIndex}].QuestionId";
        const string message = "Unknown question";
        var validationFailure = new ValidationFailure(propertyName, message, answer.QuestionId);

        return new ValidationResult(new[] { validationFailure });
    }
}
