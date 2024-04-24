import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  PollingStationInformationAPIPayload,
  upsertPollingStationGeneralInformation,
  PollingStationInformationAPIResponse,
} from "./definitions.api";
import { pollingStationsKeys } from "./queries.service";
import { useMemo } from "react";

export const useMutatePollingStationGeneralData = ({
  electionRoundId,
  pollingStationId,
  scopeId,
}: {
  electionRoundId: string | undefined;
  pollingStationId: string | undefined;
  scopeId: string;
}) => {
  const queryClient = useQueryClient();

  const pollingStationInformationQK = useMemo(
    () => pollingStationsKeys.pollingStationInformation(electionRoundId, pollingStationId),
    [electionRoundId, pollingStationId],
  );

  return useMutation({
    mutationKey: [pollingStationsKeys.mutatePollingStationGeneralData()],
    mutationFn: async (payload: PollingStationInformationAPIPayload) => {
      // TODO: RQ refactor, allow only variables here
      return upsertPollingStationGeneralInformation(payload);
    },
    scope: {
      id: scopeId,
    },
    onMutate: async ({
      electionRoundId: _electionRoundId,
      ...payload
    }: PollingStationInformationAPIPayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: pollingStationInformationQK });

      // Snapshot the previous value
      const previousData = queryClient.getQueryData<PollingStationInformationAPIResponse>(
        pollingStationInformationQK,
      );

      // In case there is no other existing value in cache
      const defaultValues = {
        id: "-1",
        pollingStationId: payload.pollingStationId,
        arrivalTime: "",
        departureTime: "",
        answers: [],
      };

      // Optimistically update to the new value
      queryClient.setQueryData<PollingStationInformationAPIResponse>(pollingStationInformationQK, {
        ...(previousData || defaultValues),
        ...(payload?.answers ? { answers: payload?.answers } : {}),
        ...(payload?.arrivalTime ? { arrivalTime: payload?.arrivalTime } : {}),
        ...(payload?.departureTime ? { departureTime: payload?.departureTime } : {}),
        // TODO: change with ...payload but first remove | null posibility from APIPayload
      });

      // Return a context object with the snapshotted value
      return { previousData };
    },
    onError: (err, newData, context) => {
      console.log(err);
      queryClient.setQueryData(pollingStationInformationQK, context?.previousData);
    },
    onSettled: () => {
      // TODO: we want to keep the mutation in pending until the refetch is done?
      return queryClient.invalidateQueries({ queryKey: pollingStationInformationQK }); // TODO: RQ
    },
  });
};
