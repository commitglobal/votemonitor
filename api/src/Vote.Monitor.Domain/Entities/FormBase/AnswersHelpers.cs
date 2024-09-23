using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormBase;

public class AnswersHelpers
{
    public static LanguagesTranslationStatus ComputeLanguagesTranslationStatus(IEnumerable<BaseQuestion> questions,
        string defaultLanguage, IEnumerable<string> languages)
    {
        var questionsArray = questions.ToArray();
        var languagesArray = languages.ToArray();

        var languagesTranslationStatus = new LanguagesTranslationStatus();

        foreach (var languageCode in languagesArray)
        {
            var status =
                questionsArray.Any(x =>
                    x.GetTranslationStatus(defaultLanguage, languageCode) == TranslationStatus.MissingTranslations)
                    ? TranslationStatus.MissingTranslations
                    : TranslationStatus.Translated;

            languagesTranslationStatus.AddOrUpdateTranslationStatus(languageCode, status);
        }

        return languagesTranslationStatus;
    }


    public static int CountNumberOfFlaggedAnswers(IEnumerable<BaseQuestion> questions, IEnumerable<BaseAnswer> answers)
    {
        var questionsArray = questions.ToArray();
        var answersArray = answers.ToArray();

        var singleSelectQuestions =
            questionsArray
                .OfType<SingleSelectQuestion>()
                .ToList();

        var multiSelectQuestions =
            questionsArray
                .OfType<MultiSelectQuestion>()
                .ToList();

        int flaggedAnswers = 0;
        foreach (var singleSelectAnswer in answersArray.OfType<SingleSelectAnswer>())
        {
            var option = singleSelectQuestions
                .FirstOrDefault(x => x.Id == singleSelectAnswer.QuestionId)
                ?.Options
                ?.FirstOrDefault(x => x.Id == singleSelectAnswer.Selection.OptionId);

            // Just in case 
            if (option is null)
            {
                continue;
            }

            if (option.IsFlagged)
            {
                flaggedAnswers++;
            }
        }

        foreach (var multiSelectAnswer in answersArray.OfType<MultiSelectAnswer>())
        {
            var options = multiSelectQuestions
                .FirstOrDefault(x => x.Id == multiSelectAnswer.QuestionId)
                ?.Options
                ?.Where(x => multiSelectAnswer.Selection.Select(o => o.OptionId).Contains(x.Id));

            // Just in case 
            if (options is null)
            {
                continue;
            }

            flaggedAnswers += options.Count(x => x.IsFlagged);
        }

        return flaggedAnswers;
    }

    public static int CountNumberOfQuestionsAnswered(IEnumerable<BaseQuestion> questions, List<BaseAnswer> answers)
    {
        var questionIds = questions.Select(x => x.Id).ToList();

        return answers.Count(x => questionIds.Contains(x.QuestionId));
    }
}