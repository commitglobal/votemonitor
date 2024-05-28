import axios from "axios";
import { FileMetadata } from "../../../hooks/useCamera";
import API from "../../api";
import { QuickReportAttachmentAPIResponse } from "./get-quick-reports.api";

/** ========================================================================
    ================= POST addAttachmentQuickReport ====================
    ========================================================================
    @description Sends a photo/video to the backend to be saved
    @param {AddAttachmentQuickReportAPIPayload} payload 
    @returns {AddAttachmentQuickReportAPIResponse} 
*/
export type AddAttachmentQuickReportAPIPayload = {
  electionRoundId: string;
  quickReportId: string;
  id: string;
  fileMetadata: FileMetadata;
};

export type AddAttachmentQuickReportAPIResponse = QuickReportAttachmentAPIResponse;

export const addAttachmentQuickReport = ({
  electionRoundId,
  quickReportId,
  id,
  fileMetadata,
}: AddAttachmentQuickReportAPIPayload): Promise<AddAttachmentQuickReportAPIResponse> => {
  const formData = new FormData();

  formData.append("attachment", {
    uri: fileMetadata.uri,
    name: fileMetadata.name,
    type: fileMetadata.type,
  } as unknown as Blob);

  formData.append("id", id);

  return API.postForm(
    `election-rounds/${electionRoundId}/quick-reports/${quickReportId}/attachments`,
    formData,
    {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    },
  ).then((res) => res.data);
};

export const addAttachmentQuickReportMultipartStart = ({
  electionRoundId,
  quickReportId,
  id,
  fileMetadata,
}: AddAttachmentQuickReportAPIPayload): Promise<any> => {
  const filePartsNo = Math.ceil(fileMetadata.size! / (10 * 1024 * 1024));

  return API.post(
    `election-rounds/${electionRoundId}/quick-reports/${quickReportId}/attachments`,
    { fileMimeType: fileMetadata.type, fileName: fileMetadata.name, filePartsNo },
    {},
  ).then((res) => res.data);
};

export const addAttachmentQuickReportMultipartComplete = async (
  uploadId: string,
  key: string,
  fileName: string,
  uploadedParts: { ETag: string; PartNumber: number }[],
): Promise<string[]> => {
  return axios
    .post(
      `https://72eb-79-115-230-202.ngrok-free.app/dossier/${145}/file/complete`,
      { uploadId, key, fileName, uploadedParts },
      {},
    )
    .then((res) => res.data);
};

export const addAttachmentQuickReportMultipartAbort = async (
  uploadId: string,
  key: string,
): Promise<string[]> => {
  return axios
    .post(
      `https://72eb-79-115-230-202.ngrok-free.app/dossier/${145}/file/abort`,
      { uploadId, key },
      {},
    )
    .then((res) => res.data);
};

// Upload S3 Chunk of bytes (Buffer (array of bytes) - not Base64 - still bytes but written differently)
export const uploadChunkDirectly = async (url: string, chunk: any): Promise<{ ETag: string }> => {
  return axios
    .put(url, chunk, {
      timeout: 100000,
      headers: {
        "Content-Type": "application/json",
      },
    })
    .then((res) => {
      return { ETag: res.headers["etag"] };
    });
};
