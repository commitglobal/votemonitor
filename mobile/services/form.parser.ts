import { FormListItem } from "../components/FormList";
import { FormAPIModel, FormSubmissionsApiResponse } from "./definitions.api";
import { ApiFormAnswer, FormQuestionAnswerTypeMapping } from "./interfaces/answer.type";
import { ApiFormQuestion, FormQuestionType } from "./interfaces/question.type";

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
  forms: FormAPIModel[],
  formSubmissions: FormSubmissionsApiResponse,
): FormListItem[] => {
  return forms.map((form) => {
    const numberOfAnswers =
      formSubmissions?.submissions.find((sub) => sub.formId === form.id)?.answers.length || 0;
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
