import { electionRoundId } from "@/lib/utils";
import { API } from "./api";

export type InitiateAttachmentMultipartUploadRequest = {
  id: string;
  citizenReportId: string;
  formId: string;
  questionId: string;
  fileName: string;
  contentType: string;
};

export type CompleteAttachmentMultipartUploadRequest = {
  uploadId: string;
  id: string;
  etags: Record<string, string>;
  citizenReportId: string;
};

export type AbortAttachmentMultipartUploadRequest = {
  uploadId: string;
  id: string;
  citizenReportId: string;
};

export const initiateAttachmentMultipartUpload = ({
  id,
  citizenReportId,
  formId,
  questionId,
  fileName,
  contentType,
}: InitiateAttachmentMultipartUploadRequest): Promise<{
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
      numberOfUploadParts: 1,
    },
    {}
  ).then((res) => res.data);
};

export const completeAttachmentMultipartUpload = async ({
  uploadId,
  id,
  etags,
  citizenReportId,
}: CompleteAttachmentMultipartUploadRequest): Promise<string[]> => {
  return API.post(
    `election-rounds/${electionRoundId}/citizen-reports/${citizenReportId}/attachments/${id}:complete`,
    { uploadId, etags },
    {}
  ).then((res) => res.data);
};

export const abortAttachmentMultipartUpload = async ({
  uploadId,
  id,
  citizenReportId,
}: AbortAttachmentMultipartUploadRequest): Promise<string[]> => {
  return API.post(
    `election-rounds/${electionRoundId}/citizen-reports/${citizenReportId}/attachments/${id}:abort`,
    { uploadId },
    {}
  ).then((res) => res.data);
};
