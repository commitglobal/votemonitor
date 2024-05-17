import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  FormSubmissionAPIPayload,
  FormSubmissionsApiResponse,
  upsertFormSubmission,
} from "../definitions.api";
import { useMemo } from "react";
import { pollingStationsKeys } from "../queries.service";
import * as Crypto from "expo-crypto";

export const useFormSubmissionMutation = ({
  electionRoundId,
  pollingStationId,
  scopeId,
}: {
  electionRoundId: string | undefined;
  pollingStationId: string | undefined;
  scopeId: string;
}) => {
  const queryClient = useQueryClient();

  const formSubmissionsQK = useMemo(
    () => pollingStationsKeys.formSubmissions(electionRoundId, pollingStationId),
    [electionRoundId, pollingStationId],
  );

  // console.log("🚬 ScopeID: ", scopeId);

  return useMutation({
    mutationKey: pollingStationsKeys.upsertFormSubmission(),
    mutationFn: async (payload: FormSubmissionAPIPayload) => {
      return upsertFormSubmission(payload);
    },
    scope: {
      id: scopeId,
    },
    onMutate: async (payload: FormSubmissionAPIPayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: formSubmissionsQK });

      // Snapshot the previous value
      const previousData = queryClient.getQueryData<FormSubmissionsApiResponse>(formSubmissionsQK);

      // Optimistically update to the new valuer
      if (payload.answers) {
        const updatedSubmission = previousData?.submissions?.find(
          (s) => s.formId === payload.formId,
        );

        queryClient.setQueryData<FormSubmissionsApiResponse>(formSubmissionsQK, {
          submissions: [
            ...(previousData?.submissions?.filter((s) => s.formId !== payload.formId) || []),
            {
              ...payload,
              id: updatedSubmission?.id || Crypto.randomUUID(),
            },
          ],
        });
        return;
      }

      // Return a context object with the snapshotted value
      return { previousData };
    },
    onError: (err, newData, context) => {
      console.log(err);
      queryClient.setQueryData(formSubmissionsQK, context?.previousData);
    },
    onSettled: () => {
      // TODO: we want to keep the mutation in pending until the refetch is done?
      return queryClient.invalidateQueries({ queryKey: formSubmissionsQK });
    },
  });
};
