import { FileMetadata } from "../../../hooks/useCamera";
import API from "../../api";
import { IncidentReportAttachmentAPIResponse } from "./get-incident-reports.api";

/** ========================================================================
    ================= POST addAttachmentQuickReport ====================
    ========================================================================
    @description Sends a photo/video to the backend to be saved
    @param {AddAttachmentQuickReportAPIPayload} payload
    @returns {AddAttachmentQuickReportAPIResponse}
*/
export type AddAttachmentQuickReportAPIPayload = {
  electionRoundId: string;
  incidentReportId: string;
  id: string;
  fileMetadata: FileMetadata;
};

export type AddAttachmentQuickReportAPIResponse = IncidentReportAttachmentAPIResponse;

export const addAttachmentQuickReport = ({
  electionRoundId,
  incidentReportId,
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
    `election-rounds/${electionRoundId}/incident-reports/${incidentReportId}/attachments`,
    formData,
    {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    },
  ).then((res) => res.data);
};
