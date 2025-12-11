import { useMutation } from '@tanstack/react-query'
import { queryClient } from '@/main'
import { formSubmissionKyes } from '@/queries/form-submissions'
import { updateFormSubmissionFollowUpStatus } from '@/services/api/form-submissions/update-status.api'
import type { FormSubmissionFollowUpStatus } from '@/types/forms-submission'

export const useUpdateFormSubmissionFollowUpStatusMutation = () =>
  useMutation({
    mutationFn: async ({
      electionRoundId,
      formSubmissionId,
      followUpStatus,
    }: {
      electionRoundId: string
      formSubmissionId: string
      followUpStatus: FormSubmissionFollowUpStatus
    }) =>
      await updateFormSubmissionFollowUpStatus(
        electionRoundId,
        formSubmissionId,
        followUpStatus
      ),
    onSuccess: async (_, { electionRoundId }) => {
      await queryClient.invalidateQueries({
        queryKey: formSubmissionKyes.all(electionRoundId),
      })
    },
  })
