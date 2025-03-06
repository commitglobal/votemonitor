import API from "../../api";
import { QuickReportAttachmentAPIResponse } from "./get-quick-reports.api";

/** ========================================================================
    ================= POST addAttachmentQuickReport ====================
    ========================================================================
    @description Sends a photo/video to the backend to be saved
    @param {AddAttachmentQuickReportStartAPIPayload} payload 
    @returns {AddAttachmentQuickReportAPIResponse} 
*/
export type AddAttachmentQuickReportStartAPIPayload = {
  electionRoundId: string;
  quickReportId: string;
  id: string;
  fileName: string;
  filePath: string;
  contentType: string;
  numberOfUploadParts: number;
};

export type AddAttachmentQuickReportCompleteAPIPayload = {
  electionRoundId: string;
  id: string;
  quickReportId: string;
  uploadId: string;
  etags: Record<number, string>;
};

export type AddAttachmentQuickReportAbortAPIPayload = {
  electionRoundId: string;
  id: string;
  quickReportId: string;
  uploadId: string;
};

export type AddAttachmentQuickReportAPIResponse = QuickReportAttachmentAPIResponse;

// Multipart Upload - Add Attachment - Question
export const addAttachmentQuickReportMultipartStart = ({
  electionRoundId,
  id,
  quickReportId,
  fileName,
  contentType,
  numberOfUploadParts,
}: AddAttachmentQuickReportStartAPIPayload): Promise<{
  uploadId: string;
  uploadUrls: Record<string, string>;
}> => {
  return API.post(
    `election-rounds/${electionRoundId}/quick-reports/${quickReportId}/attachments/${id}:init`,
    {
      fileName,
      contentType,
      numberOfUploadParts,
    },
    {},
  ).then((res) => res.data);
};

export const addAttachmentQuickReportMultipartComplete = async ({
  uploadId,
  id,
  etags,
  electionRoundId,
  quickReportId,
}: AddAttachmentQuickReportCompleteAPIPayload): Promise<string[]> => {
  return API.post(
    `election-rounds/${electionRoundId}/quick-reports/${quickReportId}/attachments/${id}:complete`,
    { uploadId, etags },
    {},
  ).then((res) => res.data);
};

export const addAttachmentQuickReportMultipartAbort = async ({
  uploadId,
  id,
  electionRoundId,
  quickReportId,
}: AddAttachmentQuickReportAbortAPIPayload): Promise<string[]> => {
  return API.post(
    `election-rounds/${electionRoundId}/quick-reports/${quickReportId}/attachments/${id}:abort`,
    { uploadId },
    {},
  ).then((res) => res.data);
};
