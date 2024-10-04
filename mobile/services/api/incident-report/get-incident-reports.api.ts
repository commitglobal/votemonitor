import API from "../../api";
import { ReportType } from "../../definitions.api";
import { ApiFormAnswer } from "../../interfaces/answer.type";
import { IncidentReportLocationType } from "./post-incident-report.api";

/** ========================================================================
    ================= GET incidentReports ====================
    ========================================================================
    @description Retrieves all Incident Reports for an Election Round ID
    @param {string} electionRoundId
    @returns {IncidentReportAPIResponse}
*/
export type IncidentReportAttachmentAPIResponse = {
  id: string;
  incidentReportId: string;
  electionRoundId: string;
  fileName: string;
  mimeType: string;
  presignedUrl: string;
  urlValidityInSeconds: number;
};

export type IncidentReport = {
  id: string;
  formId: string;
  type: ReportType;
  electionRoundId: string;
  locationType: IncidentReportLocationType;
  pollingStationId?: string | null;
  locationDescription?: string;
  attachments: Array<IncidentReportAttachmentAPIResponse>;
  answers: Array<ApiFormAnswer>;
  timestamp: string;
};

export type IncidentReportAPIResponse = {
  incidentReports: IncidentReport[];
};

export const getIncidentReports = (electionRoundId: string): Promise<IncidentReportAPIResponse> => {
  return API.get<IncidentReportAPIResponse>(
    `election-rounds/${electionRoundId}/incident-reports:my`,
  ).then((res) => ({
    incidentReports: res.data.incidentReports.map((report) => ({
      ...report,
      type: ReportType.IncidentReport,
    })),
  }));
};
