import { buildURLSearchParams } from "@/lib/utils";
import API from "@/services/api";
import type { PageResponse } from "@/types/common";
import type {
  FormSubmissionModel,
  FormSubmissionsSearch,
} from "@/types/forms-submission";

export const listFormSubmissionsByEntry = async (
  electionRoundId: string,
  search: FormSubmissionsSearch
): Promise<PageResponse<FormSubmissionModel>> => {
  return API.get(
    `/election-rounds/${electionRoundId}/form-submissions:byEntry`,
    {
      params: buildURLSearchParams(search),
    }
  ).then((res) => res.data);
};
