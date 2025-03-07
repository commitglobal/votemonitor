import { FileMetadata } from "../../hooks/useCamera";
import API from "../api";
import axios from "axios";

/** ========================================================================
    ================= POST addAttachment ====================
    ========================================================================
    @description Sends a photo/video to the backend to be saved
    @param {AddAttachmentStartAPIPayload} payload 
    @returns {AddAttachmentAPIResponse} 
*/
export type AddAttachmentStartAPIPayload = {
  id: string;
  filePath: string;
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
  fileName: string;
  contentType: string;
  numberOfUploadParts: number;
};

export type AddAttachmentCompleteAPIPayload = {
  electionRoundId: string;
  id: string;
  uploadId: string;
  etags: Record<number, string>;
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

export type AddAttachmentMultipartStartAPIResponse = {
  uploadId: string;
  uploadUrls: Record<string, string>;
};

export type AttachmentData = {
  fileMetadata: FileMetadata;
  id: string;
  uploaded: boolean;
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
}: AddAttachmentStartAPIPayload): Promise<AddAttachmentMultipartStartAPIResponse> => {
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
      lastUpdatedAt: new Date().toISOString(),
    },
    {},
  ).then((res) => res.data);
};

export const addAttachmentMultipartComplete = async ({
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

export const addAttachmentMultipartAbort = async ({
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

// Upload S3 Chunk of bytes (Buffer (array of bytes) - not Base64 - still bytes but written differently)
export const uploadS3Chunk = async (url: string, chunk: any): Promise<{ ETag: string }> => {
  return axios
    .put(url, chunk, {
      timeout: 100000,
      headers: {
        "Content-Type": "application/json",
      },
    })
    .then((res) => {
      return { ETag: res.headers.etag };
    });
};
