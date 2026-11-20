import type { FormResponseModel } from "@/common/types";
import { electionRoundId } from "@/lib/utils";
import { API } from "./api";

export const submitForm = (data: FormResponseModel) => {
  return API.post(
    `/api/election-rounds/${electionRoundId}/citizen-reports`,
    data
  ).then((res) => res.data);
};
