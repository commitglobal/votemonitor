import { useMutation, useQueryClient } from "@tanstack/react-query";
import { DeletePollingStationPayload, deletePollingStation } from "../definitions.api";
import { pollingStationsKeys } from "../queries.service";
import { useMemo } from "react";

export const useDeletePollingStationMutation = (electionRoundId: string | undefined) => {
  const queryClient = useQueryClient();
  const getPollingStationsQK = useMemo(
    () => pollingStationsKeys.visits(electionRoundId),
    [electionRoundId],
  );

  return useMutation({
    mutationKey: pollingStationsKeys.deletePollingStation(),
    mutationFn: async (payload: DeletePollingStationPayload) => {
      return deletePollingStation(payload);
    },
    onError: (err) => {
      console.log("🏠🏠🏠 ERROR IN DELETE POLLING STATION MUTATION 🏠🏠🏠", err);
    },
    onSettled: () => {
      return queryClient.invalidateQueries({
        queryKey: getPollingStationsQK,
      });
    },
  });
};
