import { useMutation } from '@tanstack/react-query'
import { queryClient } from '@/main'
import { quickReportKeys } from '@/queries/quick-reports'
import { updateQuickReportFollowUpStatus } from '@/services/api/quick-reports/update-status.api'
import type { QuickReportFollowUpStatus } from '@/types/quick-reports'

export const useUpdateQuickReportFollowUpStatusMutation = () =>
  useMutation({
    mutationFn: async ({
      electionRoundId,
      quickReportId,
      followUpStatus,
    }: {
      electionRoundId: string
      quickReportId: string
      followUpStatus: QuickReportFollowUpStatus
    }) =>
      await updateQuickReportFollowUpStatus(
        electionRoundId,
        quickReportId,
        followUpStatus
      ),
    onSuccess: async (_, { electionRoundId }) => {
      await queryClient.invalidateQueries({
        queryKey: quickReportKeys.all(electionRoundId),
      })
    },
  })
