import API from "../../api";
import { ElectionRoundsAllFormsAPIResponse } from "../../definitions.api";

export const getCitizenReportingForms = (
  electionRoundId: string,
): Promise<ElectionRoundsAllFormsAPIResponse> => {
  return API.get(`/election-rounds/${electionRoundId}/citizen-reporting-forms`).then(
    (res) => res.data,
  );
};
