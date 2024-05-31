import { useMutation, useQueryClient } from "@tanstack/react-query";
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

export const handleChunkUpload = async (
  electionRoundId: string,
  filePath: string,
  uploadUrls: Record<string, string>,
  uploadId: string,
  attachmentId: string,
) => {
  try {
    console.log("Handle chunk upload");

    let etags: Record<number, string> = {};
    const urls = Object.values(uploadUrls);
    for (const [index, url] of urls.entries()) {
      const chunk = await FileSystem.readAsStringAsync(filePath, {
        length: MULTIPART_FILE_UPLOAD_SIZE,
        position: index * MULTIPART_FILE_UPLOAD_SIZE,
        encoding: FileSystem.EncodingType.Base64,
      });
      const buffer = Buffer.from(chunk, "base64");
      const data = await uploadS3Chunk(url, buffer);
      etags = { ...etags, [index + 1]: data.ETag };
    }

    await addAttachmentMultipartComplete({
      uploadId,
      etags,
      electionRoundId,
      id: attachmentId,
    });
  } catch (err) {
    console.log(err);
    Sentry.captureMessage("Upload failed, aborting!");
    Sentry.captureException(err);
    await addAttachmentMultipartAbort({
      id: attachmentId,
      uploadId,
      electionRoundId,
    });
  }
};

// Multipart Upload - Start
export const useUploadAttachmentMutation = (scopeId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: AttachmentsKeys.addAttachmentMutation(),
    scope: {
      id: scopeId,
    },
    mutationFn: (payload: AddAttachmentStartAPIPayload) => addAttachmentMultipartStart(payload),
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
      console.log(err);
      const attachmentsQK = AttachmentsKeys.attachments(
        payload.electionRoundId,
        payload.pollingStationId,
        payload.formId,
      );
      queryClient.setQueryData(attachmentsQK, context?.previousData);
    },
    onSettled: (_data, _err, _variables) => {
      // return queryClient.invalidateQueries({
      //   queryKey: AttachmentsKeys.attachments(
      //     variables.electionRoundId,
      //     variables.pollingStationId,
      //     variables.formId,
      //   ),
      // });
    },
  });
};
