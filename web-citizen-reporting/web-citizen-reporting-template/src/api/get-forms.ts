import type { FormModel } from "@/common/types";
import { electionRoundId } from "@/lib/utils";
import { API } from "./api";

export const getForms = (): Promise<Array<FormModel>> => {
  return API.get(
    `/api/election-rounds/${electionRoundId}/citizen-reporting-forms`
  ).then((res) => res.data.forms);
};
