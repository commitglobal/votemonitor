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
  answer: string | number,
  target: string,
  condition: DisplayLogicCondition,
) => {
  // numbers and ratings compare as numbers ("10" > "9"); text compares as text
  const numeric = typeof answer === "number";
  const operand1 = answer;
  const operand2 = numeric ? Number(target) : target;
  if (numeric && Number.isNaN(operand2)) return false;
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

const pad = (n: number) => String(n).padStart(2, "0");

/** The picked local day as "YYYY-MM-DDT00:00:00Z": the API keeps the date part as is, with no time zone shift. */
export const toDayIso = (date: Date) =>
  `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T00:00:00Z`;

/** A date answer back to a local Date on that day (only the first 10 characters count). */
export const fromDayIso = (value: string) => {
  const [year, month, day] = value.slice(0, 10).split("-").map(Number);
  return new Date(year, month - 1, day);
};

/** The picked local time as "HH:mm:ss" (a time of day, without a date or time zone). */
export const toTimeOfDay = (date: Date) => `${pad(date.getHours())}:${pad(date.getMinutes())}:00`;

/** A time answer back to a Date today at that time, for the picker. */
export const fromTimeOfDay = (value: string) => {
  const [hours, minutes] = value.split(":").map(Number);
  const date = new Date();
  date.setHours(hours || 0, minutes || 0, 0, 0);
  return date;
};

/**
 * Whether a select parent's picked options meet a display rule. The rule lists the options in
 * `optionIds`: "AnyOf" needs one of them picked, "All" every one of them.
 * A condition this version doesn't know shows the question, so its answer is never discarded.
 */
const matchesSelectCondition = (
  picked: string[],
  logic: { condition: DisplayLogicCondition; value?: string | null; optionIds?: string[] | null },
) => {
  const targets = logic.optionIds ?? [];
  switch (logic.condition) {
    case "AnyOf":
      return picked.some((id) => targets.includes(id));
    case "All":
      return targets.length > 0 && targets.every((id) => picked.includes(id));
    default:
      // older rules name a single option in value: shown when that option is picked
      return !!logic.value && picked.includes(logic.value);
  }
};

/** What an answer says, for comparing two answers (ignores stamps and display-only fields). */
const answerValueKey = (answer: ApiFormAnswer): string => {
  switch (answer.$answerType) {
    case "textAnswer":
      return `text:${answer.text}`;
    case "numberAnswer":
    case "ratingAnswer":
      return `${answer.$answerType}:${answer.value}`;
    case "dateAnswer":
      return `date:${answer.date?.slice(0, 10)}`;
    case "timeAnswer":
      return `time:${answer.time?.slice(0, 5)}`;
    case "dateTimeAnswer":
      return `dateTime:${new Date(answer.dateTime).getTime()}`;
    case "singleSelectAnswer":
      return `single:${answer.selection?.optionId}:${answer.selection?.text ?? ""}`;
    case "multiSelectAnswer":
      return `multi:${(answer.selection ?? [])
        .map((option) => `${option.optionId}:${option.text ?? ""}`)
        .sort()
        .join("|")}`;
    default:
      return JSON.stringify(answer);
  }
};

/**
 * Stamps an answer with when it was first given and last changed. An unchanged answer keeps its
 * stamps, so re-saving a form doesn't move them; a changed one keeps its createdAt.
 */
export const withAnswerTimestamps = (
  next: ApiFormAnswer | undefined,
  previous: ApiFormAnswer | undefined,
  now: string = new Date().toISOString(),
): ApiFormAnswer | undefined => {
  if (!next) return undefined;
  if (previous && answerValueKey(previous) === answerValueKey(next)) {
    return { ...next, createdAt: previous.createdAt, lastUpdatedAt: previous.lastUpdatedAt };
  }
  return { ...next, createdAt: previous?.createdAt ?? now, lastUpdatedAt: now };
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
    const value = q.displayLogic?.value ?? "";
    const optionIds = q.displayLogic?.optionIds ?? [];

    let shouldDisplay = false;

    if (!condition || !parentAnswerType || (!value && optionIds.length === 0)) {
      return shouldDisplay;
    }

    switch (parentAnswerType) {
      case "multiSelectAnswer":
        shouldDisplay = matchesSelectCondition(
          (parentAnswer as MultiSelectAnswer).selection.map((option) => option.optionId),
          q.displayLogic,
        );
        break;
      case "singleSelectAnswer":
        shouldDisplay = matchesSelectCondition(
          [(parentAnswer as SingleSelectAnswer).selection.optionId],
          q.displayLogic,
        );
        break;
      case "numberAnswer":
        shouldDisplay = mapDisplayConditionToMath(
          (parentAnswer as NumberAnswer).value,
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
          (parentAnswer as RatingAnswer).value,
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

/**
 * Removes answers of questions that are no longer displayed given the current answers.
 * Questions are processed in form order so hiding a parent also hides its descendants.
 */
export const removeAnswersForHiddenQuestions = (
  questions: ApiFormQuestion[] = [],
  answers: Record<string, ApiFormAnswer | undefined>,
): Record<string, ApiFormAnswer | undefined> => {
  const updatedAnswers = { ...answers };

  questions.forEach((q) => {
    if (q.displayLogic && !shouldDisplayQuestion(q, updatedAnswers)) {
      delete updatedAnswers[q.id];
    }
  });

  return updatedAnswers;
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
        date: toDayIso(new Date(answer as string)),
      } as ApiFormAnswer;
    case "timeAnswer":
      return {
        $answerType: "timeAnswer",
        questionId,
        time: toTimeOfDay(new Date(answer as string)),
      } as ApiFormAnswer;
    case "dateTimeAnswer":
      return {
        $answerType: "dateTimeAnswer",
        questionId,
        dateTime: new Date(answer as string).toISOString(),
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
      return { [questionId]: answer.date ? fromDayIso(answer.date) : "" };
    case "timeAnswer":
      return { [questionId]: answer.time ? fromTimeOfDay(answer.time) : "" };
    case "dateTimeAnswer":
      return { [questionId]: answer.dateTime ? new Date(answer.dateTime) : "" };
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
