import { skipToken, useQuery } from "@tanstack/react-query";
import { pollingStationsKeys } from "../queries.service";
import { FormSubmissionsApiResponse, getFormSubmissions } from "../definitions.api";
import { useCallback } from "react";
import { arrayToKeyObject } from "../../helpers/misc";

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

export const useFormAnswers = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string | undefined,
) =>
  useFormSubmissions(
    electionRoundId,
    pollingStationId,
    useCallback(
      (data: FormSubmissionsApiResponse) => mapFormSubmissionToAnswersDictionary(data, formId),
      [electionRoundId, pollingStationId, formId],
    ),
  );

/** *  *  *  *  *  *  *  *  *  *  *  *  *  *
 *  *  *  *  *  *  HELPERS  *  *  *  *  *  *
 *  *  *  *  *  *  *  *  *  *  *  *  *  *  *
 */

const mapFormSubmissionToAnswersDictionary = (
  data: FormSubmissionsApiResponse,
  formId: string | undefined,
) => {
  console.log("mapFormSubmissionToAnswersDictionary");
  const formSubmission = data.submissions?.find((sub) => sub.formId === formId);
  return arrayToKeyObject(formSubmission?.answers || [], "questionId");
};
