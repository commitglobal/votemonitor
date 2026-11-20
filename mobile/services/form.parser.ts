import * as Crypto from "expo-crypto";
import i18n from "../common/config/i18n";
import { FormListItem } from "../components/SingleSubmissionFormList";
import { MultiSubmissionFormListItem } from "../components/MultiSubmissionFormList";
import { FormSubmissionDetails } from "../components/FormSubmissionListItem";
import { arrayToKeyObject, groupArrayByKey } from "../helpers/misc";
import {
  fromDayIso,
  fromTimeOfDay,
  toDateTimeIso,
  toDayIso,
  toTimeOfDay,
} from "../common/utils/date-answers";
import { FormAPIModel, FormSubmission, FormSubmissionsApiResponse } from "./definitions.api";
import { ApiFormAnswer, FormQuestionAnswerTypeMapping } from "./interfaces/answer.type";
import { ApiFormQuestion, FormQuestionType } from "./interfaces/question.type";
import { shouldDisplayQuestion } from "./display-logic";
import { getUnansweredQuestions } from "./unanswered-questions";

export { shouldDisplayQuestion } from "./display-logic";

/** What an answer says, for comparing two answers (ignores stamps and display-only fields). */
const answerValueKey = (answer: ApiFormAnswer): string => {
  switch (answer.$answerType) {
    case "textAnswer":
      return `text:${answer.text}`;
    case "numberAnswer":
    case "ratingAnswer":
      return `${answer.$answerType}:${answer.value}`;
    case "dateAnswer":
      return `date:${answer.date ? toDayIso(fromDayIso(answer.date)) : ""}`;
    case "timeAnswer":
      return `time:${answer.time?.slice(0, 5)}`;
    case "dateTimeAnswer":
      // to the minute, like what is sent: an older answer with seconds is not a change
      return `dateTime:${toDateTimeIso(new Date(answer.dateTime))}`;
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
    // date, time and date-time questions
    | Date
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
        date: toDayIso(new Date(answer as string | Date)),
      } as ApiFormAnswer;
    case "timeAnswer":
      return {
        $answerType: "timeAnswer",
        questionId,
        time: toTimeOfDay(new Date(answer as string | Date)),
      } as ApiFormAnswer;
    case "dateTimeAnswer":
      return {
        $answerType: "dateTimeAnswer",
        questionId,
        dateTime: toDateTimeIso(new Date(answer as string | Date)),
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
    // a form not started yet is not warned about: only a submission with answers, or marked as done
    const isStarted = numberOfAnswers > 0 || !!submissions[form.id]?.isCompleted;
    const unanswered = isStarted ? getUnansweredQuestions(form.questions, answers) : [];

    return {
      id: form.id,
      submissionId: submissions[form.id]?.id ?? Crypto.randomUUID(),
      numberOfUnansweredQuestions: unanswered.length,
      firstUnansweredQuestionId: unanswered[0]?.id,
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
        const unanswered = getUnansweredQuestions(form.questions, answers);
        return {
          ...submission,
          numberOfUnansweredQuestions: unanswered.length,
          firstUnansweredQuestionId: unanswered[0]?.id,
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
