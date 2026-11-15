import { FileMetadata } from "../../hooks/useCamera";
import API from "../api";
import { File, Paths } from "expo-file-system";
import { FileSystemUploadType, uploadAsync } from "expo-file-system/legacy";

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
  submissionId: string;
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
  id,
  submissionId,
  questionId,
  fileName,
  contentType,
  numberOfUploadParts,
}: AddAttachmentStartAPIPayload): Promise<AddAttachmentMultipartStartAPIResponse> => {
  return API.post(
    `election-rounds/${electionRoundId}/form-submissions/${submissionId}/attachments:init`,
    {
      electionRoundId,
      id,
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

// Upload one part of a multipart upload straight from disk. The bytes are uploaded natively by
// FileSystem.uploadAsync, so the chunk never goes through JS as a base64 string (which needed
// ~3-4x the chunk size in JS memory and crashed low-end phones on video uploads).
export type UploadS3ChunkPayload = {
  url: string;
  filePath: string;
  partIndex: number;
  partSize: number;
};

export const uploadS3Chunk = async ({
  url,
  filePath,
  partIndex,
  partSize,
}: UploadS3ChunkPayload): Promise<{ ETag: string }> => {
  const source = new File(filePath);
  const fileSize = source.size;
  const start = partIndex * partSize;
  const isWholeFile = start === 0 && fileSize <= partSize;

  // Single-part files are uploaded as-is. For multi-part, copy the byte range to a temp file
  // (native buffer, no base64) and upload that.
  let partFile: File | null = null;
  if (!isWholeFile) {
    const handle = source.open();
    try {
      handle.offset = start;
      const bytes = handle.readBytes(Math.min(partSize, fileSize - start));
      partFile = new File(Paths.cache, `upload-part-${Date.now()}-${partIndex}`);
      partFile.create({ overwrite: true });
      partFile.write(bytes);
    } finally {
      handle.close();
    }
  }

  try {
    const response = await uploadAsync(url, partFile ? partFile.uri : source.uri, {
      httpMethod: "PUT",
      uploadType: FileSystemUploadType.BINARY_CONTENT,
      headers: { "Content-Type": "application/octet-stream" },
    });

    if (response.status < 200 || response.status >= 300) {
      throw new Error(`S3 part upload failed with status ${response.status}`);
    }

    const etagKey = Object.keys(response.headers).find((key) => key.toLowerCase() === "etag");
    const ETag = etagKey ? response.headers[etagKey] : undefined;
    if (!ETag) {
      throw new Error("S3 part upload response has no ETag header");
    }
    return { ETag };
  } finally {
    if (partFile?.exists) {
      partFile.delete();
    }
  }
};
