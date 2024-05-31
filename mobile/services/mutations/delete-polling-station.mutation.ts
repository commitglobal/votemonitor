import { useMutation, useQueryClient } from "@tanstack/react-query";
import { DeletePollingStationVisitPayload, deletePollingStationVisit } from "../definitions.api";
import { pollingStationsKeys } from "../queries.service";
import { useMemo } from "react";

export const useDeletePollingStationVisitMutation = (electionRoundId: string | undefined) => {
  const queryClient = useQueryClient();
  const getPollingStationsQK = useMemo(
    () => pollingStationsKeys.visits(electionRoundId),
    [electionRoundId],
  );

  return useMutation({
    mutationKey: pollingStationsKeys.deletePollingStation(),
    mutationFn: async (payload: DeletePollingStationVisitPayload) => {
      return deletePollingStationVisit(payload);
    },
    onError: (err) => {
      console.log("🏠🏠🏠 ERROR IN DELETE POLLING STATION MUTATION 🏠🏠🏠", err);
    },
    onSuccess: () => {
      return queryClient.invalidateQueries({
        queryKey: getPollingStationsQK,
      });
    },
  });
};
