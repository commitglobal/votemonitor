import { useMutation } from "@tanstack/react-query";
import API from "../../services/api";

interface FormData {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

// Mutation Function
const changePassword = async (data: FormData) => {
  return API.post("auth/change-password", {
    password: data.currentPassword,
    newPassword: data.newPassword,
    confirmNewPassword: data.confirmPassword,
  }).catch((err) => {
    throw err;
  });
};

export const useChangePasswordMutation = () => {
  return useMutation({
    mutationKey: ["changePassword"],
    mutationFn: changePassword,
    onError: (err) => {
      //   setCredentialsError(true);
      console.log(err);
    },
    onSuccess: () => {
      //   setSuccesfullyChanged(true);
    },
  });
};
