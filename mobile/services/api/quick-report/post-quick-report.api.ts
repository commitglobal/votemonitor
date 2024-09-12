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

export enum QuickReportOfficialComplaintFilingStatus {
  Yes = 'Yes',
  NoButPlanningToFile = 'NoButPlanningToFile',
  NoAndNotPlanningToFile = 'NoAndNotPlanningToFile',
  DoesNotApplyOrOther = 'DoesNotApplyOrOther',
}

export enum QuickReportIssueType {
  A = 'A',
  B = 'B',
  C = 'C',
  D = 'D',
}

export type AddQuickReportAPIPayload = {
  id: string;
  electionRoundId: string;

  title: string;
  description: string;

  quickReportLocationType: QuickReportLocationType;
  issueType: QuickReportIssueType;
  officialComplaintFilingStatus: QuickReportOfficialComplaintFilingStatus;

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
