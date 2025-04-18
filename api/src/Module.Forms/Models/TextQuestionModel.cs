﻿using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Models;

public class TextQuestionModel : BaseQuestionModel
{
    public TranslatedString? InputPlaceholder { get; init; }

    public static TextQuestionModel FromEntity(TextQuestion question) =>
        new()
        {
            Id = question.Id,
            Code = question.Code,
            Text = question.Text,
            InputPlaceholder = question.InputPlaceholder,
            Helptext = question.Helptext,
            DisplayLogic = DisplayLogicModel.FromEntity(question.DisplayLogic)
        };
}
