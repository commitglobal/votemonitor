import { CameraResult } from "../../hooks/useCamera";
import API from "../api";

/** ========================================================================
    ================= POST addAttachment ====================
    ========================================================================
    @description Sends a photo/video to the backend to be saved
    @param {AddAttachmentAPIPayload} payload 
    @returns {AddAttachmentAPIResponse} 
*/
export type AddAttachmentAPIPayload = {
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
  cameraResult: CameraResult;
};

export type AddAttachmentAPIResponse = {
  id: string;
  fileName: string;
  mimeType: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
};

export const addAttachment = ({
  electionRoundId,
  pollingStationId,
  cameraResult,
  formId,
  questionId,
}: AddAttachmentAPIPayload): Promise<AddAttachmentAPIResponse> => {
  const formData = new FormData();

  formData.append("attachment", {
    uri: cameraResult.uri,
    name: cameraResult.name,
    type: cameraResult.type,
  } as unknown as Blob);

  formData.append("pollingStationId", pollingStationId);
  formData.append("formId", formId);
  formData.append("questionId", questionId);

  return API.postForm(`election-rounds/${electionRoundId}/attachments`, formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  }).then((res) => res.data);
};
