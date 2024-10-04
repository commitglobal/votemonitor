import API from "../../api";
import { ApiFormAnswer } from "../../interfaces/answer.type";
import { IncidentReportAPIResponse } from "./get-incident-reports.api";

/** ========================================================================
    ================= POST incidentReport ====================
    ========================================================================
    @description Upsert a Incident Report
    @param {IncidentReportAPIPayload} payload
*/

export enum IncidentReportLocationType{
  PollingStation ='PollingStation',
  OtherLocation = 'OtherLocation'
}
export type IncidentReportAPIPayload = {
  id: string;
  electionRoundId: string;
  formId: string;
  answers: ApiFormAnswer[];

  locationType: IncidentReportLocationType;
  pollingStationId?: string;
  locationDescription?: string;
};


export const upsertIncidentReport = ({
  electionRoundId,
  ...payload
}: IncidentReportAPIPayload): Promise<IncidentReportAPIResponse> => {
  return API.post(`election-rounds/${electionRoundId}/incident-reports`, payload).then(
    (res) => res.data,
  );
};
