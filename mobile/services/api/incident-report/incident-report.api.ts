import API from "../../api";
import { ApiFormAnswer } from "../../interfaces/answer.type";

export enum IncidentReportLocationType {
  NotRelatedToAPollingStation = "NotRelatedToAPollingStation",
  OtherPollingStation = "OtherPollingStation",
  PollingStation = "PollingStation",
}

export type IncidentReportAttachmentAPIResponse = {
  submissionId: string;
  questionId: string;
  fileName: string;
  mimeType: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
  uploadedFileName?: string;
  filePath?: string;
  timeSubmitted?: string;
};

export type IncidentReportNoteAPIResponse = {
  // missing from servers that predate it
  id?: string;
  submissionId: string;
  questionId: string;
  text: string;
  timeSubmitted: string;
};

export type IncidentReportAPIResponse = {
  id: string;
  formId: string;
  locationType: IncidentReportLocationType;
  pollingStationId?: string | null;
  locationDescription?: string | null;
  answers: ApiFormAnswer[];
  notes: IncidentReportNoteAPIResponse[];
  attachments: IncidentReportAttachmentAPIResponse[];
  timestamp?: string;
  isCompleted: boolean;
  isNotSynched?: boolean;
};

/** ========================================================================
    ================= GET my incident reports ====================
    ========================================================================
    @description Retrieves the incident reports of the current observer for an election round
*/
export const getMyIncidentReports = (
  electionRoundId: string,
): Promise<IncidentReportAPIResponse[]> => {
  return API.get<{ incidentReports: IncidentReportAPIResponse[] }>(
    `election-rounds/${electionRoundId}/incident-reports:my`,
  ).then((res) => res.data.incidentReports);
};

/** ========================================================================
    ================= POST incidentReport ====================
    ========================================================================
    @description Upsert an Incident Report (a submission of an IncidentReporting form)
*/
export type UpsertIncidentReportAPIPayload = {
  id: string;
  electionRoundId: string;
  formId: string;
  locationType: IncidentReportLocationType;
  pollingStationId?: string;
  locationDescription?: string;
  answers: ApiFormAnswer[];
  isCompleted?: boolean;
  lastUpdatedAt: string;
};

export const upsertIncidentReport = ({
  electionRoundId,
  ...payload
}: UpsertIncidentReportAPIPayload): Promise<IncidentReportAPIResponse> => {
  return API.post(`election-rounds/${electionRoundId}/incident-reports`, payload).then(
    (res) => res.data,
  );
};

/** ========================================================================
    ================= Incident report attachments (multipart upload) ======
    ========================================================================
*/
export type AddAttachmentIncidentReportStartAPIPayload = {
  id: string;
  electionRoundId: string;
  incidentReportId: string;
  formId: string;
  questionId: string;
  fileName: string;
  contentType: string;
  numberOfUploadParts: number;
};

export type AddAttachmentIncidentReportCompleteAPIPayload = {
  uploadId: string;
  id: string;
  etags: Record<number, string>;
  incidentReportId: string;
  electionRoundId: string;
};

export type AddAttachmentIncidentReportAbortAPIPayload = {
  uploadId: string;
  id: string;
  incidentReportId: string;
  electionRoundId: string;
};

export const addAttachmentIncidentReportMultipartStart = ({
  electionRoundId,
  incidentReportId,
  ...payload
}: AddAttachmentIncidentReportStartAPIPayload): Promise<{
  uploadId: string;
  uploadUrls: Record<string, string>;
}> => {
  return API.post(
    `election-rounds/${electionRoundId}/incident-reports/${incidentReportId}/attachments:init`,
    payload,
  ).then((res) => res.data);
};

export const addAttachmentIncidentReportMultipartComplete = ({
  uploadId,
  id,
  etags,
  incidentReportId,
  electionRoundId,
}: AddAttachmentIncidentReportCompleteAPIPayload): Promise<unknown> => {
  return API.post(
    `election-rounds/${electionRoundId}/incident-reports/${incidentReportId}/attachments/${id}:complete`,
    { uploadId, etags },
  ).then((res) => res.data);
};

export const addAttachmentIncidentReportMultipartAbort = ({
  uploadId,
  id,
  incidentReportId,
  electionRoundId,
}: AddAttachmentIncidentReportAbortAPIPayload): Promise<unknown> => {
  return API.post(
    `election-rounds/${electionRoundId}/incident-reports/${incidentReportId}/attachments/${id}:abort`,
    { uploadId },
  ).then((res) => res.data);
};

/** ========================================================================
    ================= POST incidentReportNote ====================
    ========================================================================
    @description Upsert a note on a question of an Incident Report (the report has to exist)
*/
export type UpsertIncidentReportNoteAPIPayload = {
  id: string;
  electionRoundId: string;
  incidentReportId: string;
  formId: string;
  questionId: string;
  text: string;
};

export const upsertIncidentReportNote = ({
  electionRoundId,
  incidentReportId,
  ...payload
}: UpsertIncidentReportNoteAPIPayload): Promise<unknown> => {
  return API.post(
    `election-rounds/${electionRoundId}/incident-reports/${incidentReportId}/notes`,
    payload,
  ).then((res) => res.data);
};

export type DeleteIncidentReportNoteAPIPayload = {
  id: string;
  electionRoundId: string;
  incidentReportId: string;
};

export const deleteIncidentReportNote = ({
  id,
  electionRoundId,
  incidentReportId,
}: DeleteIncidentReportNoteAPIPayload): Promise<unknown> => {
  return API.delete(
    `election-rounds/${electionRoundId}/incident-reports/${incidentReportId}/notes/${id}`,
  ).then((res) => res.data);
};

/** ========================================================================
    ================= GET / DELETE incident report attachments ============
    ========================================================================
    @description Unlike the attachments in incident-reports:my, these have their ids (needed to delete them)
*/
export type IncidentReportAttachmentWithIdAPIResponse = {
  id: string;
  incidentReportId: string;
  formId: string;
  questionId: string;
  fileName: string;
  mimeType: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
};

export const getIncidentReportAttachments = (
  electionRoundId: string,
  incidentReportId: string,
  formId: string,
): Promise<IncidentReportAttachmentWithIdAPIResponse[]> => {
  return API.get<{ attachments: IncidentReportAttachmentWithIdAPIResponse[] }>(
    `election-rounds/${electionRoundId}/incident-reports/${incidentReportId}/attachments`,
    { params: { formId } },
  ).then((res) => res.data.attachments);
};

export const deleteIncidentReportAttachment = ({
  id,
  electionRoundId,
  incidentReportId,
}: {
  id: string;
  electionRoundId: string;
  incidentReportId: string;
}): Promise<unknown> => {
  return API.delete(
    `election-rounds/${electionRoundId}/incident-reports/${incidentReportId}/attachments/${id}`,
  ).then((res) => res.data);
};
