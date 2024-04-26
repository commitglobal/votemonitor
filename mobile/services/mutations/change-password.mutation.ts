import { useMutation } from "@tanstack/react-query";
import { changePassword, ChangePasswordPayload } from "../definitions.api";

export const useChangePasswordMutation = () => {
  return useMutation({
    mutationKey: ["changePassword"],
    mutationFn: async (payload: ChangePasswordPayload) => {
      return changePassword(payload);
    },
    onError: (err) => {
      console.log("🔒🔒 ERROR 🔒🔒", err);
    },
    onSuccess: () => {
      console.log("Password changed successfully");
    },
  });
};
