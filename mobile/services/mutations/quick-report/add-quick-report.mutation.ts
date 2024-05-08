import { useMutation, useQueryClient } from "@tanstack/react-query";
import { QuickReportKeys } from "../../queries/quick-reports.query";
import { QuickReportsAPIResponse } from "../../api/quick-report/get-quick-reports.api";
import { AddQuickReportAPIPayload } from "../../api/quick-report/post-quick-report.api";

export const useAddQuickReport = (electionRoundId: string | undefined) => {
  const queryClient = useQueryClient();

  console.log("electionRoundId", electionRoundId);

  return useMutation({
    mutationKey: QuickReportKeys.add(),
    onMutate: async (payload: AddQuickReportAPIPayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)

      const queryKey = QuickReportKeys.byElectionRound(payload.electionRoundId);

      await queryClient.cancelQueries({ queryKey });

      // Snapshot the previous value
      const previousReports = queryClient.getQueryData(queryKey);

      // Optimistically update to the new value
      queryClient.setQueryData<QuickReportsAPIResponse[]>(
        queryKey,
        (old: QuickReportsAPIResponse[] = []) => [
          ...old,
          {
            ...payload,
            attachments: [],
            isNotSynched: true,
          },
        ],
      );

      //   // Return a context object with the snapshotted value
      return { previousReports };
    },
    onError: (err) => {
      console.log("🔴🔴🔴 ERROR IN ADD QUICK REPORT MUTATION 🔴🔴🔴", err);
    },
    onSettled: (_data, _err, variables: AddQuickReportAPIPayload) => {
      const queryKey = QuickReportKeys.byElectionRound(variables.electionRoundId);
      return queryClient.invalidateQueries({ queryKey });
    },
  });
};
