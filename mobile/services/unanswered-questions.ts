/*
  Unanswered questions: the ones a submission is shown with a warning for, and that the warning
  leads to. A question counts when it is
  - shown (its display logic is met: a hidden question is never asked),
  - of a type this version can show (an unknown type can't be answered here),
  - and has no answer, or an empty one (blank text, no option picked, no date / time / number).
*/
import { FormQuestionAnswerTypeMapping } from "./interfaces/answer.type";
import type { ApiFormAnswer } from "./interfaces/answer.type";
import type { ApiFormQuestion } from "./interfaces/question.type";
import { shouldDisplayQuestion } from "./display-logic";

type Answers = Record<string, ApiFormAnswer | undefined> | undefined;

/** Whether an answer says nothing (e.g. saved before it was cleared). */
export const isEmptyAnswer = (answer: ApiFormAnswer | undefined): boolean => {
  if (!answer) return true;
  switch (answer.$answerType) {
    case "textAnswer":
      return !answer.text?.trim();
    case "numberAnswer":
    case "ratingAnswer":
      return answer.value === null || answer.value === undefined || Number.isNaN(answer.value);
    case "dateAnswer":
      return !answer.date;
    case "timeAnswer":
      return !answer.time;
    case "dateTimeAnswer":
      return !answer.dateTime;
    case "singleSelectAnswer":
      return !answer.selection?.optionId;
    case "multiSelectAnswer":
      return !answer.selection?.length;
    default:
      // an answer type this version doesn't know: it says something, it just can't be shown
      return false;
  }
};

const canBeAnswered = (question: ApiFormQuestion) =>
  question.$questionType in FormQuestionAnswerTypeMapping;

/** The unanswered questions of a submission, in form order. */
export const getUnansweredQuestions = (
  questions: ApiFormQuestion[] | undefined,
  answers: Answers,
): ApiFormQuestion[] =>
  (questions ?? []).filter(
    (question) =>
      canBeAnswered(question) &&
      shouldDisplayQuestion(question, answers) &&
      isEmptyAnswer(answers?.[question.id]),
  );

/** The first unanswered question, where the warning leads to; undefined when all are answered. */
export const getFirstUnansweredQuestion = (
  questions: ApiFormQuestion[] | undefined,
  answers: Answers,
): ApiFormQuestion | undefined => getUnansweredQuestions(questions, answers)[0];

/**
 * Where to resume filling in a form: the first unanswered question (where the warning leads), or,
 * when all are answered, the last question shown. When display logic hides every question, the
 * form's last question; undefined for a form with no questions.
 */
export const getResumeQuestion = (
  questions: ApiFormQuestion[] | undefined,
  answers: Answers,
): ApiFormQuestion | undefined => {
  const list = questions ?? [];
  return (
    getFirstUnansweredQuestion(list, answers) ??
    [...list].reverse().find((question) => shouldDisplayQuestion(question, answers)) ??
    list[list.length - 1]
  );
};

/** Answers as the API lists them, keyed by question id. */
export const answersByQuestionId = (answers: ApiFormAnswer[] | undefined): Answers =>
  Object.fromEntries((answers ?? []).map((answer) => [answer.questionId, answer]));
