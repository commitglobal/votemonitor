import API from "../api";

type GetAttachmentsApiPayload = {
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
};

export type AttachmentApiResponse = {
  id: string;
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
  fileName: string;
  mimeType: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
};

export const getAttachments = ({
  electionRoundId,
  ...params
}: GetAttachmentsApiPayload): Promise<AttachmentApiResponse[]> => {
  return API.get(`election-rounds/${electionRoundId}/attachments`, {
    params,
  }).then((res) => res.data);
};
