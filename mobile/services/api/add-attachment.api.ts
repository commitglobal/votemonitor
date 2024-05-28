import { number } from "zod";
import { FileMetadata } from "../../hooks/useCamera";
import API from "../api";

/** ========================================================================
    ================= POST addAttachment ====================
    ========================================================================
    @description Sends a photo/video to the backend to be saved
    @param {AddAttachmentStartAPIPayload} payload 
    @returns {AddAttachmentAPIResponse} 
*/
export type AddAttachmentStartAPIPayload = {
  id: string;
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
  fileMetadata: FileMetadata;
  fileName: string;
  contentType: string;
  numberOfUploadParts: number;
};

export type AddAttachmentCompleteAPIPayload = {
  electionRoundId: string;
  id: string;
  uploadId: string;
  etags: string[];
};

export type AddAttachmentAbortAPIPayload = {
  electionRoundId: string;
  id: string;
  uploadId: string;
};

export type AddAttachmentAPIResponse = {
  id: string;
  fileName: string;
  mimeType: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
};

export const addAttachment = ({
  id,
  electionRoundId,
  pollingStationId,
  fileMetadata: cameraResult,
  formId,
  questionId,
}: AddAttachmentStartAPIPayload): Promise<AddAttachmentAPIResponse> => {
  const formData = new FormData();

  formData.append("attachment", {
    uri: cameraResult.uri,
    name: cameraResult.name,
    type: cameraResult.type,
  } as unknown as Blob);

  formData.append("id", id);
  formData.append("pollingStationId", pollingStationId);
  formData.append("formId", formId);
  formData.append("questionId", questionId);

  return API.postForm(`election-rounds/${electionRoundId}/attachments`, formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  }).then((res) => res.data);
};

// Multipart Upload - Add Attachment - Question
export const addAttachmentMultipartStart = ({
  electionRoundId,
  pollingStationId,
  id,
  formId,
  questionId,
  fileName,
  contentType,
  numberOfUploadParts,
}: AddAttachmentStartAPIPayload): Promise<any> => {
  return API.post(
    `election-rounds/${electionRoundId}/attachments:init`,
    {
      pollingStationId,
      electionRoundId,
      id,
      formId,
      questionId,
      fileName,
      contentType,
      numberOfUploadParts,
    },
    {},
  ).then((res) => res.data);
};

export const addAttachmenttMultipartComplete = async ({
  uploadId,
  id,
  etags,
  electionRoundId,
}: AddAttachmentCompleteAPIPayload): Promise<string[]> => {
  return API.post(
    `election-rounds/${electionRoundId}/attachments/${id}:complete`,
    { uploadId, etags },
    {},
  ).then((res) => res.data);
};

export const addAttachmenttMultipartAbort = async ({
  uploadId,
  id,
  electionRoundId,
}: AddAttachmentAbortAPIPayload): Promise<string[]> => {
  return API.post(
    `election-rounds/${electionRoundId}/attachments/${id}:abort`,
    { uploadId },
    {},
  ).then((res) => res.data);
};
