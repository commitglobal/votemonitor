import API from "../../api";
import { ReportType } from "../../definitions.api";
import { QuickReportLocationType } from "./post-quick-report.api";

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
  type: ReportType;
  id: string;
  electionRoundId: string;
  quickReportLocationType: QuickReportLocationType;
  title: string;
  description: string;
  pollingStationId?: string | null;
  pollingStationDetails?: string;
  attachments: Array<QuickReportAttachmentAPIResponse>;
  timestamp: string;
};

export const getQuickReports = (
  electionRoundId: string,
): Promise<Array<QuickReportsAPIResponse>> => {
  return API.get<Array<QuickReportsAPIResponse>>(
    `election-rounds/${electionRoundId}/quick-reports:my`,
  ).then((res) =>
    res.data.map((qr) => ({
      ...qr,
      type: ReportType.QuickReport,
    })),
  );
};
