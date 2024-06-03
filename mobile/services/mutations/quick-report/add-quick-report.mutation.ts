import { useMutation, useQueryClient } from "@tanstack/react-query";
import { QuickReportKeys } from "../../queries/quick-reports.query";
import {
  QuickReportAttachmentAPIResponse,
  QuickReportsAPIResponse,
} from "../../api/quick-report/get-quick-reports.api";
import { AddQuickReportAPIPayload } from "../../api/quick-report/post-quick-report.api";
import { AddAttachmentQuickReportStartAPIPayload } from "../../api/quick-report/add-attachment-quick-report.api";

export const useAddQuickReport = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: QuickReportKeys.add(),
    onMutate: async ({
      attachments,
      ...payload
    }: AddQuickReportAPIPayload & { attachments: AddAttachmentQuickReportStartAPIPayload[] }) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      const queryKey = QuickReportKeys.byElectionRound(payload.electionRoundId);
      await queryClient.cancelQueries({ queryKey });

      // Snapshot the previous value
      const previousReports = queryClient.getQueryData(queryKey);

      const attachmentsToUpdate: QuickReportAttachmentAPIResponse[] = attachments.map((attach) => {
        return {
          electionRoundId: attach.electionRoundId,
          fileName: attach.fileName,
          id: attach.id,
          mimeType: attach.contentType,
          presignedUrl: attach.filePath,
          quickReportId: attach.quickReportId,
          urlValidityInSeconds: 0,
        };
      });

      // Optimistically update to the new value
      queryClient.setQueryData<QuickReportsAPIResponse[]>(
        queryKey,
        (old: QuickReportsAPIResponse[] = []) => [
          ...old,
          {
            ...payload,
            attachments: attachmentsToUpdate,
            isNotSynched: true,
          },
        ],
      );

      //   // Return a context object with the snapshotted value
      return { previousReports };
    },
    onError: (err, variables, context) => {
      console.log("🔴🔴🔴 ERROR IN ADD QUICK REPORT MUTATION 🔴🔴🔴", err);
      const queryKey = QuickReportKeys.byElectionRound(variables.electionRoundId);
      queryClient.setQueryData(queryKey, context?.previousReports);
    },
    onSettled: (
      _data,
      _err,
      variables: AddQuickReportAPIPayload & {
        attachments: AddAttachmentQuickReportStartAPIPayload[];
      },
    ) => {
      const queryKey = QuickReportKeys.byElectionRound(variables.electionRoundId);
      return queryClient.invalidateQueries({ queryKey });
    },
  });
};
