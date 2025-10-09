import { updateFormSubmissionFollowUpStatus } from "@/services/api/form-submissions/update-status.api";
import type { FormSubmissionFollowUpStatus } from "@/types/forms-submission";
import { useMutation } from "@tanstack/react-query";

export const useUpdateFormSubmissionFollowUpStatusMutation = () =>
  useMutation({
    mutationFn: async ({
      electionRoundId,
      formSubmissionId,
      followUpStatus,
    }: {
      electionRoundId: string;
      formSubmissionId: string;
      followUpStatus: FormSubmissionFollowUpStatus;
    }) =>
      await updateFormSubmissionFollowUpStatus(
        electionRoundId,
        formSubmissionId,
        followUpStatus
      ),
  });
