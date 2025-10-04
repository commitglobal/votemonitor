using AngleSharp.Text;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Vote.Monitor.Ingestor.Core.Converters;

public class SmsToFormSubmissionDecoder
{
    private const int FORM_CODE_START_INDEX = 0;
    private const int FORM_CODE_LENGTH = 2;
    private const int CORRELATION_ID_START_INDEX = 2;
    private const int CORRELATION_ID_LENGTH = 4;
    private const int POLLING_STATION_CODE_START_INDEX = 6;
    private const int POLLING_STATION_CODE_LENGTH = 4;
    private const int QUESTIONS_START_INDEX = 10;
    private const int QUESTION_CODE_LENGTH = 2;

    public SmsFormSubmission Decode(string smsMessage)
    {
        ValidateSmsMessage(smsMessage);

        var formCode = ExtractFormCode(smsMessage);
        ValidateFormCode(formCode);

        var correlationId = ExtractCorrelationId(smsMessage);
        ValidateCorrelationId(correlationId);

        var pollingStationCode = ExtractPollingStationCode(smsMessage);
        ValidatePollingStationCode(pollingStationCode);

        var questions = ExtractQuestions(smsMessage);
        ValidateQuestions(questions);

        return new SmsFormSubmission() { 
            FormCode = formCode, 
            CorrelationId = correlationId, 
            PollingStationCode = pollingStationCode,
            Answers = questions
        };

    }

    private void ValidateSmsMessage(string smsMessage)
    {
        if (string.IsNullOrWhiteSpace(smsMessage)) throw new ArgumentException($"{nameof(smsMessage)} must not be null or empty");
    }

    private string ExtractFormCode(string smsMessage)
    {
        return smsMessage.Substring(FORM_CODE_START_INDEX, FORM_CODE_LENGTH);
    }

    private void ValidateFormCode(string formCode)
    {
        if (formCode.Any(c => !c.IsAlphanumericAscii()))
            throw new ArgumentException($"Form code '{formCode}' contains invalid characters. Only alphanumeric characters are valid");
    }

    private string ExtractCorrelationId(string smsMessage)
    {
        return smsMessage.Substring(CORRELATION_ID_START_INDEX, CORRELATION_ID_LENGTH);
    }

    private void ValidateCorrelationId(string correlationId)
    {
        if (correlationId.Any(c => !c.IsAlphanumericAscii()))
            throw new ArgumentException($"Correlation ID '{correlationId}' contains invalid characters. Only alphanumeric characters are valid");
    }

    private string ExtractPollingStationCode(string smsMessage)
    {
        return smsMessage.Substring(POLLING_STATION_CODE_START_INDEX, POLLING_STATION_CODE_LENGTH);
    }

    private void ValidatePollingStationCode(string pollingStationCode)
    {
        if (pollingStationCode.Any(c => !c.IsAlphanumericAscii()))
            throw new ArgumentException($"Polling station code '{pollingStationCode}' contains invalid characters. Only alphanumeric characters are valid");
    }

    private SmsFormSubmission.SmsAnswer[] ExtractQuestions(string smsMessage)
    {
        var questionsText = smsMessage.Substring(QUESTIONS_START_INDEX);

        List<SmsFormSubmission.SmsAnswer> questions = new();

        bool readingCode = true;

        string currentCode = "";
        int answerStartIndex = -1;

        for(var i = 0; i < questionsText.Length;)
        {
            if (readingCode)
            {
                currentCode = questionsText.Substring(i, QUESTION_CODE_LENGTH);
                i += QUESTION_CODE_LENGTH;
                answerStartIndex = i;
                readingCode = false;
            }
            else
            {
                if (!questionsText[i].IsLetter())
                {
                    i++;
                    continue;
                } 
                else
                {
                    var answer = questionsText.Substring(answerStartIndex, i - answerStartIndex);
                    questions.Add(new() { Code = currentCode, Value = answer });
                    answerStartIndex = -1;
                    readingCode = true;
                }
            }
        }

        if (!string.IsNullOrEmpty(currentCode))
        {
            var answer = questionsText.Substring(answerStartIndex);
            questions.Add(new() { Code = currentCode, Value = answer});
        }

        return questions.ToArray();
    }

    private void ValidateQuestions(SmsFormSubmission.SmsAnswer[] questions)
    {
        if (!questions.Any())
            throw new ArgumentException("The message must contain question answers");
        foreach(var question in questions)
        {
            if(question.Code.Any(c => !c.IsLetter()))
                throw new ArgumentException($"Question code '{question.Code}' contains invalid characters. Only letters are valid");
            if (question.Value.Any(c => !c.IsDigit()))
                throw new ArgumentException($"The answer '{question.Value}' for question '{question.Code}' contains invalid characters. Only digits are valid");
        }
    }
}

public class SmsFormSubmission
{
    public required string FormCode;
    public required string CorrelationId;
    public required string PollingStationCode;

    public required SmsAnswer[] Answers;

    public class SmsAnswer
    {
        public required string Code;
        public required string Value;
    }
}
