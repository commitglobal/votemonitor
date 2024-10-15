using System.Text;
using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class CitizenReportEmailGenerator
{
    private static readonly string Template = EmailTemplateLoader.GetTemplate(EmailTemplateType.CitizenReport);

    public static EmailModel Generate(CitizenReportEmailProps props)
    {
        var body = Template
            .Replace("~heading~", props.Heading)
            .Replace("~preview~", props.Preview)
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$answers$~", BuildAnswersFragment(props.Answers));

        return new EmailModel(props.Title, body);
    }

    private static string BuildAnswersFragment(IEnumerable<BaseAnswerFragmentProps> answers)
    {
        var userAnswers = answers as BaseAnswerFragmentProps[] ?? answers.ToArray();

        if (!userAnswers.Any()) return string.Empty;

        var result = new StringBuilder();
        foreach (var answer in userAnswers)
        {
            switch (answer)
            {
                case InputAnswerFragmentProps inputAnswer:
                    result.Append(EmailTemplateLoader
                        .GetTemplate(EmailTemplateType.InputAnswerFragment)
                        .Replace("~$text$~", inputAnswer.Text)
                        .Replace("~$answer$~", inputAnswer.Answer));
                    break;

                case RatingAnswerFragmentProps ratingAnswer:
                    result.Append(EmailTemplateLoader
                        .GetTemplate(EmailTemplateType.RatingAnswerFragment)
                        .Replace("~$text$~", ratingAnswer.Text)
                        .Replace("~$options$~", BuildRatingOptionsFragment(ratingAnswer.Scale, ratingAnswer.Value)));
                    break;

                case SelectAnswerFragmentProps selectAnswer:
                    result.Append(EmailTemplateLoader
                        .GetTemplate(EmailTemplateType.SelectAnswerFragment)
                        .Replace("~$text$~", selectAnswer.Text)
                        .Replace("~$options$~", BuildSelectOptionsFragment(selectAnswer.Options)));
                    break;
            }
        }

        return result.ToString();
    }

    private static string BuildSelectOptionsFragment(IEnumerable<SelectAnswerOptionFragmentProps> options)
    {
        var result = new StringBuilder();

        foreach (var option in options)
        {
            var optionTemplate = EmailTemplateLoader.GetTemplate(option.IsSelected
                ? EmailTemplateType.SelectAnswerCheckedOptionFragment
                : EmailTemplateType.SelectAnswerOptionFragment);

            result.Append(optionTemplate.Replace("~$value$~", option.Text));
        }

        return result.ToString();
    }

    private static string BuildRatingOptionsFragment(int scale, int value)
    {
        var result = new StringBuilder();

        for (var i = 1; i <= scale; i++)
        {
            var optionTemplate = EmailTemplateLoader.GetTemplate(i == value
                ? EmailTemplateType.RatingAnswerOptionCheckedOptionFragment
                : EmailTemplateType.RatingAnswerOptionOptionFragment);

            result.Append(optionTemplate.Replace("~$value$~", i.ToString()));
        }

        return result.ToString();
    }
}