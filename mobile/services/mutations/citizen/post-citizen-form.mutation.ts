import { useMutation } from "@tanstack/react-query";
import {
  CitizenReportFormAPIPayload,
  postCitizenReportForm,
} from "../../api/citizen/post-citizen-form";
import { MUTATION_SCOPE_DO_NOT_HYDRATE } from "../../../common/constants";

export const usePostCitizenFormMutation = () => {
  return useMutation({
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationKey: ["postCitizenReportForm"],
    mutationFn: (data: CitizenReportFormAPIPayload) => postCitizenReportForm(data),
  });
};
