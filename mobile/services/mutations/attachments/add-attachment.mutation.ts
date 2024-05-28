import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  AddAttachmentAPIPayload,
  AddAttachmentAPIResponse,
  addAttachment,
} from "../../api/add-attachment.api";
import { AttachmentApiResponse } from "../../api/get-attachments.api";
import { AttachmentsKeys } from "../../queries/attachments.query";
import {
  AddAttachmentQuickReportAPIPayload,
  addAttachmentQuickReportMultipartComplete,
  addAttachmentQuickReportMultipartStart,
  uploadChunk,
} from "../../api/quick-report/add-attachment-quick-report.api";

export const addAttachmentMutation = (scopeId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: AttachmentsKeys.addAttachmentMutation(),
    scope: {
      id: scopeId,
    },
    mutationFn: async (payload: AddAttachmentAPIPayload): Promise<AddAttachmentAPIResponse> => {
      return addAttachment(payload);
    },
    onMutate: async (payload: AddAttachmentAPIPayload) => {
      const attachmentsQK = AttachmentsKeys.attachments(
        payload.electionRoundId,
        payload.pollingStationId,
        payload.formId,
      );

      await queryClient.cancelQueries({ queryKey: attachmentsQK });

      const previousData = queryClient.getQueryData<AttachmentApiResponse[]>(attachmentsQK);

      queryClient.setQueryData<AttachmentApiResponse[]>(attachmentsQK, [
        ...(previousData || []),
        {
          id: payload.id,
          electionRoundId: payload.electionRoundId,
          pollingStationId: payload.pollingStationId,
          formId: payload.formId,
          questionId: payload.questionId,
          fileName: `${payload.fileMetadata.name}`,
          mimeType: payload.fileMetadata.type,
          presignedUrl: payload.fileMetadata.uri, // TODO @radulescuandrew is this working to display the media?
          urlValidityInSeconds: 3600,
          isNotSynched: true,
        },
      ]);

      return { previousData, attachmentsQK };
    },
    onError: (err, payload, context) => {
      console.log(err);
      const attachmentsQK = AttachmentsKeys.attachments(
        payload.electionRoundId,
        payload.pollingStationId,
        payload.formId,
      );
      queryClient.setQueryData(attachmentsQK, context?.previousData);
    },
    onSettled: (_data, _err, variables) => {
      return queryClient.invalidateQueries({
        queryKey: AttachmentsKeys.attachments(
          variables.electionRoundId,
          variables.pollingStationId,
          variables.formId,
        ),
      });
    },
  });
};

// Multipart Upload

export const useUploadAttachmentMutation = (scopeId: string) => {
  return useMutation({
    mutationKey: AttachmentsKeys.addAttachmentMutation(),
    scope: {
      id: scopeId,
    },
    mutationFn: (payload: AddAttachmentQuickReportAPIPayload) =>
      addAttachmentQuickReportMultipartStart(payload),

    onError: (error: any) => Promise.resolve(error),
  });
};

export const useUploadS3ChunkMutation = (scopeId: string) => {
  return useMutation({
    mutationKey: AttachmentsKeys.addAttachmentMutation(),
    scope: {
      id: scopeId,
    },
    mutationFn: ({ url, data }: { url: string; data: any }) => uploadChunk(url, data),
    onError: (error: any) => {
      return Promise.resolve(error);
    },
    retry: 3,
  });
};

export const useCompleteAddAttachmentUploadMutation = (scopeId: string) => {
  return useMutation({
    mutationKey: AttachmentsKeys.addAttachmentMutation(),
    scope: {
      id: scopeId,
    },
    mutationFn: ({
      uploadId,
      key,
      fileName,
      uploadedParts,
    }: {
      uploadId: string;
      key: string;
      fileName: string;
      uploadedParts: { ETag: string; PartNumber: number }[];
    }) => addAttachmentQuickReportMultipartComplete(uploadId, key, fileName, uploadedParts),
    onError: (error: any) => {
      console.log("err completing");
      return Promise.resolve(error);
    },
    retry: 3,
  });
};

// export const useAbortDossierFileUploadMutation = () => {
//   return useMutation(
//     ({ dossierId, uploadId, key }: { dossierId: number; uploadId: string; key: string }) =>
//       abortUploadDossierFile(dossierId, uploadId, key),
//     {
//       onError: (error: AxiosError<IBusinessException<DOSSIER_FILES_ERRORS>>) => {
//         console.log("err aborting");
//         return Promise.resolve(error);
//       },
//       retry: 3,
//     },
//   );
// };
