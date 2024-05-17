import { useMutation } from "@tanstack/react-query";
import { DeletePollingStationPayload, deletePollingStation } from "../definitions.api";
import { pollingStationsKeys } from "../queries.service";

export const useDeletePollingStationMutation = () => {
  return useMutation({
    mutationKey: pollingStationsKeys.deletePollingStation(),
    mutationFn: async (payload: DeletePollingStationPayload) => {
      return deletePollingStation(payload);
    },
  });
};
