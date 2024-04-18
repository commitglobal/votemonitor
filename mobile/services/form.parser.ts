import { ApiFormAnswer } from "./interfaces/answer.type";
import { ApiFormQuestion } from "./interfaces/question.type";

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
