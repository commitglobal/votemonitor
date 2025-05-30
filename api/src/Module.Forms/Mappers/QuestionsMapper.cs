﻿using Module.Forms.Models;
using Module.Forms.Requests;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Mappers;

public static class QuestionsMapper
{
    public static BaseQuestionModel ToModel(BaseQuestion question)
    {
        switch (question)
        {
            case TextQuestion textInputQuestion:
                return TextQuestionModel.FromEntity(textInputQuestion);

            case NumberQuestion numberInputQuestionRequest:
                return NumberQuestionModel.FromEntity(numberInputQuestionRequest);

            case DateQuestion dateInputQuestionRequest:
                return DateQuestionModel.FromEntity(dateInputQuestionRequest);

            case SingleSelectQuestion singleSelectQuestionRequest:
                return SingleSelectQuestionModel.FromEntity(singleSelectQuestionRequest);

            case MultiSelectQuestion multiSelectQuestionRequest:
                return MultiSelectQuestionModel.FromEntity(multiSelectQuestionRequest);

            case RatingQuestion ratingQuestionRequest:
                return RatingQuestionModel.FromEntity(ratingQuestionRequest);

            default: throw new ApplicationException("Unknown question type received");
        }
    }

    public static BaseQuestion ToEntity(BaseQuestionRequest question)
    {
        switch (question)
        {
            case TextQuestionRequest textInputQuestion:
                return TextQuestion.Create(textInputQuestion.Id,
                    textInputQuestion.Code,
                    textInputQuestion.Text,
                    textInputQuestion.Helptext,
                    textInputQuestion.InputPlaceholder,
                    ToEntity(textInputQuestion.DisplayLogic));

            case NumberQuestionRequest numberInputQuestion:
                return NumberQuestion.Create(numberInputQuestion.Id,
                     numberInputQuestion.Code,
                     numberInputQuestion.Text,
                     numberInputQuestion.Helptext,
                     numberInputQuestion.InputPlaceholder,
                     ToEntity(numberInputQuestion.DisplayLogic));

            case DateQuestionRequest dateInputQuestion:
                return DateQuestion.Create(dateInputQuestion.Id,
                    dateInputQuestion.Code,
                    dateInputQuestion.Text,
                    dateInputQuestion.Helptext,
                    ToEntity(dateInputQuestion.DisplayLogic));

            case SingleSelectQuestionRequest singleSelectQuestion:
                var singleSelectQuestionOptions = ToEntities(singleSelectQuestion.Options);

                return SingleSelectQuestion.Create(singleSelectQuestion.Id,
                    singleSelectQuestion.Code,
                    singleSelectQuestion.Text,
                    singleSelectQuestionOptions,
                    singleSelectQuestion.Helptext,
                    ToEntity(singleSelectQuestion.DisplayLogic));

            case MultiSelectQuestionRequest multiSelectQuestion:
                var multiSelectQuestionOptions = ToEntities(multiSelectQuestion.Options);

                var multiSelectQuestionEntity = MultiSelectQuestion.Create(multiSelectQuestion.Id,
                    multiSelectQuestion.Code,
                    multiSelectQuestion.Text,
                    multiSelectQuestionOptions,
                    multiSelectQuestion.Helptext,
                    ToEntity(multiSelectQuestion.DisplayLogic));

                return multiSelectQuestionEntity;

            case RatingQuestionRequest ratingQuestion:
                return RatingQuestion.Create(ratingQuestion.Id,
                     ratingQuestion.Code,
                     ratingQuestion.Text,
                     RatingScale.FromValue(ratingQuestion.Scale.Value),
                     ratingQuestion.Helptext,
                     ratingQuestion.LowerLabel,
                     ratingQuestion.UpperLabel,
                     ToEntity(ratingQuestion.DisplayLogic));

            default: throw new ApplicationException("Unknown question type received");
        }
    }

    private static IReadOnlyList<SelectOption> ToEntities(IEnumerable<SelectOptionRequest> options)
    {
        return options
            .Select(o => SelectOption.Create(o.Id, o.Text, o.IsFreeText, o.IsFlagged))
            .ToList()
            .AsReadOnly();
    }

    private static DisplayLogic? ToEntity(DisplayLogicRequest? displayLogic)
    {
        return displayLogic == null
            ? null
            : DisplayLogic.Create(displayLogic.ParentQuestionId, displayLogic.Condition, displayLogic.Value);
    }
}
