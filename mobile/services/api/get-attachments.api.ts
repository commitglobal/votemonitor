import API from "../api";

type GetAttachmentsApiPayload = {
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
};

export enum AttachmentMimeType {
  IMG = "image/jpeg",
  VIDEO = "video/mp4",
  AUDIO_MPEG = "audio/mpeg",
  AUDIO_M4A = "audio/x-m4a",
}

export type AttachmentApiResponse = {
  id: string;
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
  fileName: string;
  mimeType: AttachmentMimeType;
  presignedUrl: string;
  urlValidityInSeconds: number;
  isNotSynched?: boolean;
};

export const getAttachments = ({
  electionRoundId,
  ...params
}: GetAttachmentsApiPayload): Promise<AttachmentApiResponse[]> => {
  return API.get(`election-rounds/${electionRoundId}/attachments`, {
    params,
  }).then((res) => res.data);
};
