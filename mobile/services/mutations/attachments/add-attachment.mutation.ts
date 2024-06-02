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

export const uploadAttachmentMutationFn = async (
  payload: AddAttachmentStartAPIPayload,
  queryClient: QueryClient,
) => {
  queryClient.setQueryData(AttachmentsKeys.addAttachments(), () => ({
    progress: 0,
    status: "starting",
  }));
  const start = await addAttachmentMultipartStart(payload);
  try {
    let etags: Record<number, string> = {};
    const urls = Object.values(start.uploadUrls);

    console.log("🚀 Started the FOR LOOP FOR CHUNKS");
    for (const [index, url] of urls.entries()) {
      const chunk = await FileSystem.readAsStringAsync(payload.filePath, {
        length: MULTIPART_FILE_UPLOAD_SIZE,
        position: index * MULTIPART_FILE_UPLOAD_SIZE,
        encoding: FileSystem.EncodingType.Base64,
      });
      const buffer = Buffer.from(chunk, "base64");
      const progress = Math.round(((index + 1) / urls.length) * 100 * 10) / 10;
      queryClient.setQueryData(AttachmentsKeys.addAttachments(), () => {
        const toReturn: UploadAttachmentProgress = {
          progress,
          status: progress === 100 ? "completed" : "inprogress",
        };
        console.log("toReturnProgress in handleChunkUpload", toReturn);

        return toReturn;
      });

      console.log(
        "Current progress state:",
        queryClient.getQueryData(AttachmentsKeys.addAttachments()),
      );

      const data = await uploadS3Chunk(url, buffer);

      etags = { ...etags, [index + 1]: data.ETag };
    }
    console.log("❌ Ended the FOR LOOP FOR CHUNKS");
    const completed = await addAttachmentMultipartComplete({
      uploadId: start.uploadId,
      etags,
      electionRoundId: payload.electionRoundId,
      id: payload.id,
    });
    queryClient.setQueryData<UploadAttachmentProgress>(
      AttachmentsKeys.addAttachments(),
      (oldData) => {
        const toReturn: UploadAttachmentProgress = {
          ...oldData,
          progress: 100,
          status: "completed",
        };
        console.log("toReturnCompleted", toReturn);
        return toReturn;
      },
    );

    return completed;
  } catch (err) {
    Sentry.captureMessage("Upload failed, aborting!");
    Sentry.captureException(err);

    const aborted = addAttachmentMultipartAbort({
      id: payload.id,
      uploadId: start.uploadId,
      electionRoundId: payload.electionRoundId,
    });
    queryClient.setQueryData<UploadAttachmentProgress>(
      AttachmentsKeys.addAttachments(),
      (oldData) => {
        const toReturn: UploadAttachmentProgress = {
          ...oldData,
          progress: 0,
          status: "aborted",
        };
        console.log("toReturnAbort", toReturn);
        return toReturn;
      },
    );
    return aborted;
  }
};

export type UploadAttachmentProgress = {
  progress: number;
  status: "idle" | "compressing" | "starting" | "inprogress" | "aborted" | "completed";
};
export const useUploadAttachmentProgressQuery = () => {
  return useQuery({
    queryKey: AttachmentsKeys.addAttachments(), // TODO: more specific
    queryFn: () => {
      console.log("QueryFn Called");
      return { status: "idle", progress: 0 };
    },
    placeholderData: { status: "idle", progress: 0 },
    initialData: { status: "idle", progress: 0 },
    staleTime: Infinity,
    gcTime: Infinity,
  });
};

export const useUploadAttachmentMutation = (scopeId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: AttachmentsKeys.addAttachmentMutation(),
    scope: {
      id: scopeId,
    },
    mutationFn: (payload: AddAttachmentStartAPIPayload) =>
      uploadAttachmentMutationFn(payload, queryClient),
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
    onSettled: (_data, _err, variables) => {
      console.log("onSettled");
      // queryClient.setQueryData<UploadAttachmentProgress>(
      //   AttachmentsKeys.addAttachments(),
      //   (oldData) => {
      //     const toReturn: UploadAttachmentProgress = {
      //       ...oldData,
      //       progress: 0,
      //       status: "idle",
      //     };
      //     console.log("toReturnOnSettled", toReturn);
      //     return toReturn;
      //   },
      // );
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
