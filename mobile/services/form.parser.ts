import * as Crypto from "expo-crypto";
import i18n from "../common/config/i18n";
import { FormListItem } from "../components/SingleSubmissionFormList";
import { MultiSubmissionFormListItem } from "../components/MultiSubmissionFormList";
import { FormSubmissionDetails } from "../components/FormSubmissionListItem";
import { arrayToKeyObject, groupArrayByKey } from "../helpers/misc";
import { FormAPIModel, FormSubmission, FormSubmissionsApiResponse } from "./definitions.api";
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
  answers: Record<string, ApiFormAnswer | undefined> | undefined,
) => {
  if (q.displayLogic) {
    if (!answers?.[q.displayLogic?.parentQuestionId]) {
      return false;
    }
    const condition: DisplayLogicCondition = q.displayLogic.condition;
    const parentAnswerType = answers?.[q.displayLogic?.parentQuestionId]?.$answerType;
    const parentAnswer = answers?.[q.displayLogic?.parentQuestionId];
    const value = q.displayLogic?.value;

    let shouldDisplay = false;

    if (!condition || !parentAnswerType || !value) {
      return shouldDisplay;
    }

    switch (parentAnswerType) {
      case "multiSelectAnswer":
        shouldDisplay = !!(parentAnswer as MultiSelectAnswer).selection.find(
          (option) => option.optionId === value,
        );
        break;
      case "singleSelectAnswer":
        shouldDisplay = (parentAnswer as SingleSelectAnswer).selection.optionId === value;
        break;
      case "numberAnswer":
        shouldDisplay = mapDisplayConditionToMath(
          (parentAnswer as NumberAnswer).value.toString(),
          value,
          condition,
        );
        break;
      case "textAnswer":
        shouldDisplay = mapDisplayConditionToMath(
          (parentAnswer as TextAnswer).text,
          value,
          condition,
        );
        break;
      case "ratingAnswer":
        shouldDisplay = mapDisplayConditionToMath(
          (parentAnswer as RatingAnswer).value.toString(),
          value,
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
  answer:
    | string
    | number
    | Record<string, string>
    | Record<string, { optionId: string; text: string }>,
): ApiFormAnswer | undefined => {
  if (!answer) return undefined;

  switch (FormQuestionAnswerTypeMapping[questionType]) {
    case "numberAnswer":
      return {
        $answerType: "numberAnswer",
        questionId,
        value: +answer,
      } as ApiFormAnswer;
    case "ratingAnswer":
      return {
        $answerType: "ratingAnswer",
        questionId,
        value: +answer,
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

export enum SubmissionStatus {
  NOT_STARTED = "not started",
  IN_PROGRESS = "in progress",
  COMPLETED = "completed",
  MARKED_AS_COMPLETED = "markedAsCompleted",
}

export const mapSubmissionStateStatus = (
  numberOfAnswers: number,
  numberOfQuestions: number,
  isCompleted: boolean,
): SubmissionStatus => {
  if (isCompleted) return SubmissionStatus.MARKED_AS_COMPLETED;

  if (numberOfAnswers === 0) return SubmissionStatus.NOT_STARTED;
  if (numberOfAnswers < numberOfQuestions) return SubmissionStatus.IN_PROGRESS;
  if (numberOfAnswers === numberOfQuestions) return SubmissionStatus.COMPLETED;
  return SubmissionStatus.NOT_STARTED;
};

export const mapFormToFormListItem = (
  forms: FormAPIModel[] = [],
  formSubmissions: FormSubmissionsApiResponse | undefined,
): FormListItem[] => {
  const currentLanguage = i18n.language.toLocaleUpperCase();

  const submissions = arrayToKeyObject(formSubmissions?.submissions || [], "formId");
  return forms.map((form) => {
    const answers = arrayToKeyObject(submissions[form?.id]?.answers || [], "questionId");
    const questions = form.questions.filter((q) => shouldDisplayQuestion(q, answers));

    const numberOfAnswers = submissions[form.id]?.answers.length || 0;

    return {
      id: form.id,
      submissionId: submissions[form.id]?.id ?? Crypto.randomUUID(),
      name: `${form.code} - ${form.name[currentLanguage] || form.name[Object.keys(form?.name)[0]]}`,
      numberOfCompletedQuestions: numberOfAnswers,
      numberOfQuestions: questions.length,
      options: `Available in ${Object.keys(form.name).join(", ")}`, // TODO: localize!
      status: mapSubmissionStateStatus(
        numberOfAnswers,
        questions.length,
        submissions[form.id]?.isCompleted || false,
      ),
      languages: form.languages,
    };
  });
};
export const mapFormToMultiSubmissionFormListItem = (
  forms: FormAPIModel[] = [],
  formSubmissions: FormSubmissionsApiResponse | undefined,
): MultiSubmissionFormListItem[] => {
  const currentLanguage = i18n.language.toLocaleUpperCase();

  const submissions = groupArrayByKey(formSubmissions?.submissions || [], "formId");
  return forms.map((form) => {
    const numberOfSubmissions = submissions[form.id]?.length || 0;

    return {
      id: form.id,
      name: `${form.code} - ${form.name[currentLanguage] || form.name[Object.keys(form?.name)[0]]}`,
      options: `Available in ${Object.keys(form.name).join(", ")}`,
      numberOfSubmissions,
      languages: form.languages,
    };
  });
};

export const mapFormToFormSubmissionListItem = (
  form: FormAPIModel,
  formSubmissions: FormSubmission[] | undefined,
): FormSubmissionDetails[] => {
  return (
    formSubmissions
      ?.sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()) // ascending
      ?.map((submission, idx) => ({
        ...submission,
        submissionNumber: idx + 1,
      }))
      ?.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()) // descending
      ?.map((submission) => {
        const answers = arrayToKeyObject(submission.answers || [], "questionId");
        const questions = form.questions.filter((q) => shouldDisplayQuestion(q, answers));

        const numberOfAnswers = submission.answers.length || 0;
        return {
          ...submission,
          numberOfAttachments: submission.numberOfAttachments ?? 0,
          numberOfNotes: submission.numberOfNotes ?? 0,
          numberOfCompletedQuestions: numberOfAnswers,
          numberOfQuestions: questions.length,
          formStatus: mapSubmissionStateStatus(
            numberOfAnswers,
            questions.length,
            submission?.isCompleted || false,
          ),
        };
      }) ?? []
  );
};
