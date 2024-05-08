import { useMutation } from "@tanstack/react-query";
import { changePassword, ChangePasswordPayload } from "../definitions.api";
import { pollingStationsKeys } from "../queries.service";

export const useChangePasswordMutation = () => {
  return useMutation({
    mutationKey: pollingStationsKeys.changePassword(),
    mutationFn: async (payload: ChangePasswordPayload) => {
      return changePassword(payload);
    },
  });
};
