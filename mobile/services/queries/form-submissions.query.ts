import { skipToken, useQuery } from "@tanstack/react-query";
import { pollingStationsKeys } from "../queries.service";
import { FormSubmissionsApiResponse, getFormSubmissions } from "../definitions.api";
import { ApiFormAnswer } from "../interfaces/answer.type";

export const useFormSubmissions = <TResult = FormSubmissionsApiResponse>(
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  select?: (data: FormSubmissionsApiResponse) => TResult,
) => {
  return useQuery({
    queryKey: pollingStationsKeys.formSubmissions(electionRoundId, pollingStationId),
    queryFn:
      electionRoundId && pollingStationId
        ? () => getFormSubmissions(electionRoundId, pollingStationId)
        : skipToken,
    select,
  });
};

export const mapAPIAnswersToFormAnswers = (
  apiAnswers: ApiFormAnswer[] = [],
): Record<string, ApiFormAnswer> => {
  return apiAnswers.reduce((acc: Record<string, ApiFormAnswer>, curr: ApiFormAnswer) => {
    acc[curr.questionId] = curr;

    return acc;
  }, {});
};

export const useFormAnswers = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string | undefined,
) =>
  useFormSubmissions(electionRoundId, pollingStationId, (data: FormSubmissionsApiResponse) => {
    const formSubmission = data.submissions.find((sub) => sub.formId === formId);
    return mapAPIAnswersToFormAnswers(formSubmission?.answers);
  });
