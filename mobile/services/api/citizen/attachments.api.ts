import API from "../../api";

export type AddAttachmentCitizenStartAPIPayload = {
  id: string;
  electionRoundId: string;
  citizenReportId: string;
  formId: string;
  questionId: string;
  fileName: string;
  contentType: string;
  numberOfUploadParts: number;
};

export type AddAttachmentCitizenCompleteAPIPayload = {
  uploadId: string;
  id: string;
  etags: Record<string, string>;
  citizenReportId: string;
  electionRoundId: string;
};

export type AddAttachmentCitizenAbortAPIPayload = {
  uploadId: string;
  id: string;
  citizenReportId: string;
  electionRoundId: string;
};

// Multipart Upload - Add Attachment - Citizen Report
export const addAttachmentCitizenMultipartStart = ({
  electionRoundId,
  id,
  citizenReportId,
  formId,
  questionId,
  fileName,
  contentType,
  numberOfUploadParts,
}: AddAttachmentCitizenStartAPIPayload): Promise<{
  uploadId: string;
  uploadUrls: Record<string, string>;
}> => {
  return API.post(
    `election-rounds/${electionRoundId}/citizen-reports/${citizenReportId}/attachments:init`,
    {
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

export const addAttachmentCitizenMultipartComplete = async ({
  uploadId,
  id,
  etags,
  citizenReportId,
  electionRoundId,
}: AddAttachmentCitizenCompleteAPIPayload): Promise<string[]> => {
  return API.post(
    `election-rounds/${electionRoundId}/citizen-reports/${citizenReportId}/attachments/${id}:complete`,
    { uploadId, etags },
    {},
  ).then((res) => res.data);
};

export const addAttachmentCitizenMultipartAbort = async ({
  uploadId,
  id,
  electionRoundId,
  citizenReportId,
}: AddAttachmentCitizenAbortAPIPayload): Promise<string[]> => {
  return API.post(
    `election-rounds/${electionRoundId}/citizen-reports/${citizenReportId}/attachments/${id}:abort`,
    { uploadId },
    {},
  ).then((res) => res.data);
};
