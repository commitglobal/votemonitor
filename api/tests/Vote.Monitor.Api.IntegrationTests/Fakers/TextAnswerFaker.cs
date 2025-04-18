﻿using Module.Answers.Requests;

namespace Vote.Monitor.Api.IntegrationTests.Fakers;

public sealed class TextAnswerFaker : Faker<TextAnswerRequest>
{
    public TextAnswerFaker(Guid questionId)
    {
        RuleFor(x => x.QuestionId, questionId);
        RuleFor(x => x.Text, f => f.Lorem.Sentence(100));
    }
}
