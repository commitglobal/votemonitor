import { QueryClient, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  AddAttachmentStartAPIPayload,
  addAttachmentMultipartAbort,
  addAttachmentMultipartComplete,
  addAttachmentMultipartStart,
  uploadS3Chunk,
} from "../../api/add-attachment.api";
import { AttachmentApiResponse } from "../../api/get-attachments.api";
import { AttachmentsKeys } from "../../queries/attachments.query";
import * as FileSystem from "expo-file-system";
import { MULTIPART_FILE_UPLOAD_SIZE } from "../../../common/constants";
import * as Sentry from "@sentry/react-native";
import { Buffer } from "buffer";
// export const handleChunkUpload = async (
//   filePath: string,
//   uploadUrls: Record<string, string>,
//   queryClient: QueryClient,
// ) => {
//   console.log("Handle chunk upload");

//   let etags: Record<number, string> = {};
//   const urls = Object.values(uploadUrls);
//   for (const [index, url] of urls.entries()) {
//     const chunk = await FileSystem.readAsStringAsync(filePath, {
//       length: MULTIPART_FILE_UPLOAD_SIZE,
//       position: index * MULTIPART_FILE_UPLOAD_SIZE,
//       encoding: FileSystem.EncodingType.Base64,
//     });
//     const buffer = Buffer.from(chunk, "base64");
//     const data = await uploadS3Chunk(url, buffer);

//     const progress = Math.round(((index + 1) / urls.length) * 100 * 10) / 10;
//     queryClient.setQueryData<UploadAttachmentProgress>(
//       AttachmentsKeys.addAttachments(),
//       (oldData) => {
//         const toReturn: UploadAttachmentProgress = {
//           ...oldData,
//           progress,
//           status: progress === 100 ? "completed" : "inprogress",
//         };
//         console.log("toReturnProgress in handleChunkUpload", toReturn);

//         return toReturn;
//       },
//     );

//     etags = { ...etags, [index + 1]: data.ETag };
//   }

//   return etags;
// };

export const uploadAttachmentMutationFn = async (payload: AddAttachmentStartAPIPayload) => {
  const start = await addAttachmentMultipartStart(payload);
  try {
    let etags: Record<number, string> = {};
    const urls = Object.values(start.uploadUrls);

    for (const [index, url] of urls.entries()) {
      const chunk = await FileSystem.readAsStringAsync(payload.filePath, {
        length: MULTIPART_FILE_UPLOAD_SIZE,
        position: index * MULTIPART_FILE_UPLOAD_SIZE,
        encoding: FileSystem.EncodingType.Base64,
      });
      const buffer = Buffer.from(chunk, "base64");
      // const progress = Math.round(((index + 1) / urls.length) * 100 * 10) / 10;

      const data = await uploadS3Chunk(url, buffer);

      etags = { ...etags, [index + 1]: data.ETag };
    }
    const completed = await addAttachmentMultipartComplete({
      uploadId: start.uploadId,
      etags,
      electionRoundId: payload.electionRoundId,
      id: payload.id,
    });

    return completed;
  } catch (err) {
    Sentry.captureMessage("Upload failed, aborting!");
    Sentry.captureException(err);

    const aborted = addAttachmentMultipartAbort({
      id: payload.id,
      uploadId: start.uploadId,
      electionRoundId: payload.electionRoundId,
    });

    return aborted;
  }
};

export type UploadAttachmentProgress = {
  progress: number;
};

export const useUploadAttachmentMutation = (scopeId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: AttachmentsKeys.addAttachmentMutation(),
    scope: {
      id: scopeId,
    },
    mutationFn: (payload: AddAttachmentStartAPIPayload) => uploadAttachmentMutationFn(payload),
    onMutate: async (payload: AddAttachmentStartAPIPayload) => {
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
          fileName: `${payload.fileName}`,
          mimeType: payload.contentType,
          presignedUrl: payload.filePath,
          urlValidityInSeconds: 3600,
          isNotSynched: true,
        },
      ]);

      return { previousData, attachmentsQK };
    },
    onError: (err, payload, context) => {
      console.log("onError", err);
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
