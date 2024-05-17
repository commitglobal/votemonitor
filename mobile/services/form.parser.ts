import { FormListItem } from "../components/FormList";
import { arrayToKeyObject } from "../helpers/misc";
import { FormAPIModel, FormSubmissionsApiResponse } from "./definitions.api";
import {
  ApiFormAnswer,
  FormQuestionAnswerTypeMapping,
  MultiSelectAnswer,
  NumberAnswer,
  RatingAnswer,
  SingleSelectAnswer,
  TextAnswer,
} from "./interfaces/answer.type";
import {
  ApiFormQuestion,
  DisplayLogicCondition,
  FormQuestionType,
} from "./interfaces/question.type";

const mapDisplayConditionToMath = (
  operand1: string,
  operand2: string,
  condition: DisplayLogicCondition,
) => {
  switch (condition) {
    case "Equals":
      return operand1 === operand2;
    case "GreaterEqual":
      return operand1 >= operand2;
    case "GreaterThan":
      return operand1 > operand2;
    case "LessEqual":
      return operand1 <= operand2;
    case "LessThan":
      return operand1 < operand2;
    case "NotEquals":
      return operand1 !== operand2;
    default:
      return false;
  }
};

export const shouldDisplayQuestion = (
  q: ApiFormQuestion,
  answers: Record<string, ApiFormAnswer> | undefined,
) => {
  if (q.displayLogic) {
    if (!answers?.[q.displayLogic?.parentQuestionId]) {
      return false;
    }
    const condition: DisplayLogicCondition = q.displayLogic.condition;
    const parentAnswerType = answers?.[q.displayLogic?.parentQuestionId].$answerType;
    const parentAnswer = answers?.[q.displayLogic?.parentQuestionId];

    let shouldDisplay = false;

    switch (parentAnswerType) {
      case "multiSelectAnswer":
        shouldDisplay = !!(parentAnswer as MultiSelectAnswer).selection.find(
          (option) => option.optionId === q.displayLogic?.value,
        );
        break;
      case "singleSelectAnswer":
        shouldDisplay =
          (parentAnswer as SingleSelectAnswer).selection.optionId === q.displayLogic?.value;
        break;
      case "numberAnswer":
        shouldDisplay = mapDisplayConditionToMath(
          (parentAnswer as NumberAnswer).value.toString(),
          q.displayLogic.value,
          condition,
        );
        break;
      case "textAnswer":
        shouldDisplay = mapDisplayConditionToMath(
          (parentAnswer as TextAnswer).text,
          q.displayLogic.value,
          condition,
        );
        break;
      case "ratingAnswer":
        shouldDisplay = mapDisplayConditionToMath(
          (parentAnswer as RatingAnswer).value.toString(),
          q.displayLogic.value,
          condition,
        );
        break;
      default:
        shouldDisplay = false;
    }

    return shouldDisplay;
  }

  return true;
};

export const mapAPIQuestionsToFormQuestions = (
  apiQuestions: ApiFormQuestion[] = [],
): Record<string, ApiFormQuestion> => {
  return apiQuestions.reduce((acc: Record<string, ApiFormQuestion>, curr: ApiFormQuestion) => {
    acc[curr.id] = curr;

    return acc;
  }, {});
};

export const mapAPIAnswersToFormAnswers = (
  apiAnswers: ApiFormAnswer[] = [],
): Record<string, ApiFormAnswer> => {
  return apiAnswers.reduce((acc: Record<string, ApiFormAnswer>, curr: ApiFormAnswer) => {
    acc[curr.questionId] = curr;

    return acc;
  }, {});
};

export const mapFormSubmissionDataToAPIFormSubmissionAnswer = (
  questionId: string,
  questionType: FormQuestionType,
  answer: string | Record<string, string> | Record<string, { optionId: string; text: string }>,
): ApiFormAnswer | undefined => {
  switch (FormQuestionAnswerTypeMapping[questionType]) {
    case "numberAnswer":
      return {
        $answerType: "numberAnswer",
        questionId,
        value: answer,
      } as ApiFormAnswer;
    case "ratingAnswer":
      return {
        $answerType: "ratingAnswer",
        questionId,
        value: answer,
      } as ApiFormAnswer;
    case "textAnswer":
      return {
        $answerType: "textAnswer",
        questionId,
        text: answer,
      } as ApiFormAnswer;
    case "dateAnswer":
      return {
        $answerType: "dateAnswer",
        questionId,
        date: new Date(answer as string).toISOString(),
      } as ApiFormAnswer;
    case "singleSelectAnswer":
      return {
        $answerType: "singleSelectAnswer",
        questionId,
        selection: {
          optionId: (answer as Record<string, string>).radioValue,
          text: (answer as Record<string, string>).textValue,
        },
      } as ApiFormAnswer;
    case "multiSelectAnswer": {
      const selections: Record<string, { optionId: string; text: string }> = answer as Record<
        string,
        { optionId: string; text: string }
      >;
      return {
        $answerType: "multiSelectAnswer",
        questionId,
        selection: Object.values(selections).map((selection) => ({
          optionId: selection.optionId,
          text: selection.text,
        })),
      } as ApiFormAnswer;
    }
    default:
      return undefined;
  }
};

export const setFormDefaultValues = (questionId: string, answer?: ApiFormAnswer) => {
  if (!answer) {
    return { [questionId]: "" };
  }

  switch (answer.$answerType) {
    case "textAnswer":
      return { [questionId]: answer.text };
    case "numberAnswer":
    case "ratingAnswer":
      return { [questionId]: answer?.value?.toString() ?? "" };
    case "dateAnswer":
      return { [questionId]: answer.date ? new Date(answer.date) : "" };
    case "singleSelectAnswer":
      return {
        [questionId]: {
          radioValue: answer.selection.optionId,
          textValue: answer.selection.text,
        },
      };
    case "multiSelectAnswer":
      return {
        [questionId]: answer.selection.reduce((acc: Record<string, any>, curr) => {
          acc[curr.optionId] = { optionId: curr.optionId, text: curr.text };
          return acc;
        }, {}),
      };
    default:
      break;
  }
};

export enum FormStatus {
  NOT_STARTED = "not started",
  IN_PROGRESS = "in progress",
  COMPLETED = "completed",
}

export const mapFormStateStatus = (
  numberOfAnswers: number,
  numberOfQuestions: number,
): FormStatus => {
  if (numberOfAnswers === 0) return FormStatus.NOT_STARTED;
  if (numberOfAnswers < numberOfQuestions) return FormStatus.IN_PROGRESS;
  if (numberOfAnswers === numberOfQuestions) return FormStatus.COMPLETED;
  return FormStatus.NOT_STARTED;
};

export const mapFormToFormListItem = (
  forms: FormAPIModel[] = [],
  formSubmissions: FormSubmissionsApiResponse | undefined,
): FormListItem[] => {
  const submissions = arrayToKeyObject(formSubmissions?.submissions || [], "formId");
  return forms.map((form) => {
    const numberOfAnswers = submissions[form.id]?.answers.length || 0;
    return {
      id: form.id,
      name: `${form.code} - ${form.name.RO}`,
      numberOfCompletedQuestions: numberOfAnswers,
      numberOfQuestions: form.questions.length,
      options: `Available in ${Object.keys(form.name).join(", ")}`,
      status: mapFormStateStatus(numberOfAnswers, form.questions.length),
      languages: form.languages,
    };
  });
};
