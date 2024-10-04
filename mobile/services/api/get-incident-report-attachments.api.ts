import API from "../api";
import { AttachmentMimeType } from "./get-attachments.api";

type GetIncidentReportAttachmentsApiPayload = {
  electionRoundId: string;
  incidentReportId: string;
};

export type IncidentReportAttachmentApiResponse = {
  id: string;
  electionRoundId: string;
  incidentReportId: string;
  formId: string;
  questionId: string;
  fileName: string;
  mimeType: AttachmentMimeType;
  presignedUrl: string;
  urlValidityInSeconds: number;
  isNotSynched?: boolean;
};

export const getIncidentReportAttachments = ({
  electionRoundId,
  incidentReportId,
}: GetIncidentReportAttachmentsApiPayload): Promise<IncidentReportAttachmentApiResponse[]> => {
  return API.get(
    `election-rounds/${electionRoundId}/incident-reports/${incidentReportId}/attachments`,
  ).then((res) => res.data);
};
