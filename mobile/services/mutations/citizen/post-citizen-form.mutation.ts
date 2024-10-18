import { useMutation } from "@tanstack/react-query";
import {
  CitizenReportFormAPIPayload,
  postCitizenReportForm,
} from "../../api/citizen/post-citizen-form";

export const usePostCitizenFormMutation = () => {
  return useMutation({
    mutationKey: ["postCitizenReportForm"],
    mutationFn: (data: CitizenReportFormAPIPayload) => postCitizenReportForm(data),
  });
};
