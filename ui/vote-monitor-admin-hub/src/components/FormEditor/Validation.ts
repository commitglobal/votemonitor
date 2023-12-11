// extend this object in order to add more validation rules

import { TFormMultipleChoiceMultiQuestion, TFormMultipleChoiceSingleQuestion, TFormQuestion } from "@/redux/api/types";


const validationRules = {
  multipleChoiceMulti: (question: TFormMultipleChoiceMultiQuestion) => {
    return !question.choices.some((element) => element.label.trim() === "");
  },
  multipleChoiceSingle: (question: TFormMultipleChoiceSingleQuestion) => {
    return !question.choices.some((element) => element.label.trim() === "");
  },

  defaultValidation: (question: TFormQuestion) => {
    return question.headline.trim() !== "";
  },
};

const validateQuestion = (question: TFormQuestion) => {
  const specificValidation = validationRules[question.type];
  const defaultValidation = validationRules.defaultValidation;

  const specificValidationResult = specificValidation ? specificValidation(question) : true;
  const defaultValidationResult = defaultValidation(question);

  // Return true only if both specific and default validation pass
  return specificValidationResult && defaultValidationResult;
};

export { validateQuestion };
