import { skipToken, useMutation, useQuery } from "@tanstack/react-query";
import { pollingStationsKeys } from "../queries.service";
import {
  FormSubmissionsApiResponse,
  getFormSubmissions,
  getPSHasFormSubmissions,
} from "../definitions.api";
import { useCallback } from "react";
import { arrayToKeyObject } from "../../helpers/misc";

export const formSubmissionsQueryFn = (
  electionRoundId: string | undefined,
  pollingStationId: string,
) => getFormSubmissions(electionRoundId!, pollingStationId);

export const useFormSubmissions = <TResult = FormSubmissionsApiResponse>(
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  select?: (data: FormSubmissionsApiResponse) => TResult,
) => {
  return useQuery({
    queryKey: pollingStationsKeys.allFormSubmissions(electionRoundId, pollingStationId),
    queryFn:
      electionRoundId && pollingStationId
        ? () => formSubmissionsQueryFn(electionRoundId, pollingStationId)
        : skipToken,
    select,
  });
};

export const useFormSubmissionsByFormId = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string,
) => {
  return useFormSubmissions(
    electionRoundId,
    pollingStationId,
    useCallback(
      (data: FormSubmissionsApiResponse) => {
        const formSubmissions = data.submissions?.filter((sub) => sub.formId === formId);

        return formSubmissions;
      },
      [electionRoundId, pollingStationId, formId],
    ),
  );
};

export const useFormSubmissionById = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  submissionId: string,
) => {
  return useFormSubmissions(
    electionRoundId,
    pollingStationId,
    useCallback(
      (data: FormSubmissionsApiResponse) => {
        const formSubmission = data.submissions?.find((sub) => sub.id === submissionId);
        return {
          id: formSubmission?.id,
          answers: arrayToKeyObject(formSubmission?.answers || [], "questionId"),
          isCompleted: formSubmission?.isCompleted || false,
          createdAt: formSubmission?.createdAt,
        };
      },
      [electionRoundId, pollingStationId, submissionId],
    ),
  );
};

export const usePSHasFormSubmissions = (electionRoundId?: string, pollingStationId?: string) => {
  return useMutation({
    mutationKey: pollingStationsKeys.psHasFormSubmissions(electionRoundId, pollingStationId),
    mutationFn: ({
      electionRoundId,
      pollingStationId,
    }: {
      electionRoundId: string;
      pollingStationId: string;
    }) => {
      return getPSHasFormSubmissions(electionRoundId, pollingStationId);
    },
  });
};
