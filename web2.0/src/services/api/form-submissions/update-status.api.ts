import API from "@/services/api";
import type { FormSubmissionFollowUpStatus } from "@/types/forms-submission";

export const updateFormSubmissionFollowUpStatus = async (
  electionRoundId: string,
  formSubmissionId: string,
  followUpStatus: FormSubmissionFollowUpStatus
) => {
  return API.put<void>(
    `/election-rounds/${electionRoundId}/form-submissions/${formSubmissionId}:status`,
    {
      followUpStatus,
    }
  );
};
