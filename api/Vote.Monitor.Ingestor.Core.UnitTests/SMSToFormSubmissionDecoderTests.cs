using FluentAssertions;
using NSubstitute.ExceptionExtensions;
using Vote.Monitor.Ingestor.Core.Converters;

namespace Vote.Monitor.Ingestor.Core.UnitTests;

public class SMSToFormSubmissionDecoderTests
{
    private static SMSToFormSubmissionDecoder _decoder = new();

    private const string VALID_FORM_CODE = "A3";
    private const string FORM_CODE_WITH_INVALID_CHARACTERS = "1#";

    private const string VALID_CORRELATION_ID = "12AB";
    private const string CORRELATION_ID_WITH_INVALID_CHARACTERS = "12A!";

    private const string VALID_POLLING_STATION_CODE = "ABC3";
    private const string POLLING_STATION_CODE_WITH_INVALID_CHARACTERS = "C!#.";

    private const string VALID_QUESTION_CODE = "QA";
    private const string QUESTION_CODE_WITH_INVALID_CHARACTERS = "!2";

    private const string VALID_ANSWER = "132";
    private const string ANSWER_WITH_INVALID_CHARACTERS = "!24$";

    private static (string questionCode, string questionAnswer)[] _validQuestions = [(VALID_QUESTION_CODE, VALID_ANSWER)];

    [Fact]
    public void DecodingEmptyString_ThrowsException()
    {
        _decoder.Invoking(d => d.Decode(string.Empty)).Should().Throw<ArgumentException>().WithMessage("smsMessage must not be null or empty");
    }

    [Fact]
    public void DecodingStringWithoutValidFormCode_ThrowsException()
    {
        _decoder.Invoking(d => d.Decode(GetEncodedString(formCode: FORM_CODE_WITH_INVALID_CHARACTERS))).Should()
            .Throw<ArgumentException>()
            .WithMessage($"Form code '{FORM_CODE_WITH_INVALID_CHARACTERS}' contains invalid characters. Only alphanumeric characters are valid");
    }

    [Fact]
    public void DecodingStringWithValidFormCode_ReturnsItInASMSFormSubmission()
    {
        SMSFormSubmission smsFormSubmission = _decoder.Decode(GetEncodedString(formCode: VALID_FORM_CODE));
        smsFormSubmission.FormCode.Should().Be(VALID_FORM_CODE);
    }

    [Fact]
    public void DecodingStringWithoutValidCorrelationId_ThrowsException()
    {
        _decoder.Invoking(d => d.Decode(GetEncodedString(correlationId: CORRELATION_ID_WITH_INVALID_CHARACTERS))).Should()
            .Throw<ArgumentException>()
            .WithMessage($"Correlation ID '{CORRELATION_ID_WITH_INVALID_CHARACTERS}' contains invalid characters. Only alphanumeric characters are valid");
    }

    [Fact]
    public void DecodingStringWithValidCorrelationId_ReturnsItInASMSFormSubmission()
    {
        SMSFormSubmission smsFormSubmission = _decoder.Decode(GetEncodedString(correlationId: VALID_CORRELATION_ID));
        smsFormSubmission.CorrelationId.Should().Be(VALID_CORRELATION_ID);
    }


    [Fact]
    public void DecodingStringWithoutValidPollingStationCode_ThrowsException()
    {
        _decoder.Invoking(d => d.Decode(GetEncodedString(pollingStationCode: POLLING_STATION_CODE_WITH_INVALID_CHARACTERS))).Should()
            .Throw<ArgumentException>()
            .WithMessage($"Polling station code '{POLLING_STATION_CODE_WITH_INVALID_CHARACTERS}' contains invalid characters. Only alphanumeric characters are valid");
    }

    [Fact]
    public void DecodingStringWithValidPollingStationCode_ReturnsItInASMSFormSubmission()
    {
        SMSFormSubmission smsFormSubmission = _decoder.Decode(GetEncodedString(pollingStationCode: VALID_POLLING_STATION_CODE));
        smsFormSubmission.PollingStationCode.Should().Be(VALID_POLLING_STATION_CODE);
    }

    [Fact]
    public void DecodingStringWithoutQuestions_ThrowsException()
    {
        _decoder.Invoking(d => d.Decode(GetEncodedString(questions: []))).Should()
            .Throw<ArgumentException>()
            .WithMessage("The message must contain question answers");
    }

    [Fact]
    public void DecodingStringWithInvalidQuestionCode_ThrowsException()
    {
        _decoder.Invoking(d => d.Decode(GetEncodedString(questions: [(QUESTION_CODE_WITH_INVALID_CHARACTERS, VALID_ANSWER)]))).Should()
            .Throw<ArgumentException>()
            .WithMessage($"Question code '{QUESTION_CODE_WITH_INVALID_CHARACTERS}' contains invalid characters. Only letters are valid");
    }

    [Fact]
    public void DecodingStringWithInvalidQuestionAnswer_ThrowsException()
    {
        _decoder.Invoking(d => d.Decode(GetEncodedString(questions: [(VALID_QUESTION_CODE, ANSWER_WITH_INVALID_CHARACTERS)]))).Should()
            .Throw<ArgumentException>()
            .WithMessage($"The answer '{ANSWER_WITH_INVALID_CHARACTERS}' for question '{VALID_QUESTION_CODE}' contains invalid characters. Only digits are valid");
    }

    [Fact]
    public void DecodingStringWithValidQuestionAnswer_ReturnsThemInASMSFormSubmission()
    {
        SMSFormSubmission smsFormSubmission = _decoder.Decode(GetEncodedString(questions: [(VALID_QUESTION_CODE, VALID_ANSWER), (VALID_QUESTION_CODE, VALID_ANSWER)]));
        smsFormSubmission.Questions.Length.Should().Be(2);
        smsFormSubmission.Questions.Should().AllSatisfy(q => q.Code.Should().Be(VALID_QUESTION_CODE));
        smsFormSubmission.Questions.Should().AllSatisfy(q => q.Answer.Should().Be(VALID_ANSWER));
    }

    private static string GetEncodedString(
        string formCode = VALID_FORM_CODE,
        string correlationId = VALID_CORRELATION_ID,
        string pollingStationCode = VALID_POLLING_STATION_CODE,
        (string questionCode, string questionAnswer)[]? questions = null)
    {
        questions ??= _validQuestions;

        var encodedString = $"{formCode}{correlationId}{pollingStationCode}";

        foreach (var (questionCode, questionAnswer) in questions)
        {
            encodedString += $"{questionCode}{questionAnswer}";
        }

        return encodedString;
    }
}
