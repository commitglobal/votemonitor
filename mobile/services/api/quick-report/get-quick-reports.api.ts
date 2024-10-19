import API from "../../api";
import {} from "../../definitions.api";
import { IncidentCategory, QuickReportLocationType } from "./post-quick-report.api";

/** ========================================================================
    ================= GET quickReports ====================
    ========================================================================
    @description Retrieves all Quick Reports for an Election Round ID
    @param {string} electionRoundId
    @returns {QuickReportsAPIResponse}
*/
export type QuickReportAttachmentAPIResponse = {
  id: string;
  quickReportId: string;
  electionRoundId: string;
  fileName: string;
  mimeType: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
};
export type QuickReportsAPIResponse = {
  id: string;
  electionRoundId: string;
  quickReportLocationType: QuickReportLocationType;
  incidentCategory: IncidentCategory;
  title: string;
  description: string;
  pollingStationId?: string | null;
  pollingStationDetails?: string;
  attachments: Array<QuickReportAttachmentAPIResponse>;
};

export const getQuickReports = (
  electionRoundId: string,
): Promise<Array<QuickReportsAPIResponse>> => {
  return API.get(`election-rounds/${electionRoundId}/quick-reports:my`).then((res) => res.data);
};
