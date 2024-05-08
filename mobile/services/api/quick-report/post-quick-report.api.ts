import API from "../../api";
import { QuickReportsAPIResponse } from "./get-quick-reports.api";

/** ========================================================================
    ================= POST quickReport ====================
    ========================================================================
    @description Upsert a Quick Report
    @param {AddQuickReportAPIPayload} payload 
*/
export enum QuickReportLocationType {
  NotRelatedToAPollingStation = "NotRelatedToAPollingStation",
  OtherPollingStation = "OtherPollingStation",
  VisitedPollingStation = "VisitedPollingStation",
}
export type AddQuickReportAPIPayload = {
  id: string;
  electionRoundId: string;

  title: string;
  description: string;

  quickReportLocationType: QuickReportLocationType;
  pollingStationId?: string;
  pollingStationDetails?: string;
};

// TODO: add rreturn type
export const addQuickReport = ({
  electionRoundId,
  ...payload
}: AddQuickReportAPIPayload): Promise<QuickReportsAPIResponse> => {
  return API.post(`election-rounds/${electionRoundId}/quick-reports`, payload).then(
    (res) => res.data,
  );
};
