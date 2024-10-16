import API from "../../api";

export interface CitizenReportCopyAPIPayload {
  electionRoundId: string;
  citizenReportId: string;
  reqBody: {
    formId: string;
    email: string;
  };
}
export const putCitizenReportCopy = async ({
  electionRoundId,
  citizenReportId,
  reqBody,
}: CitizenReportCopyAPIPayload) => {
  return await API.put(
    `/election-rounds/${electionRoundId}/citizen-reports/${citizenReportId}:sendCopy`,
    reqBody,
  ).then((res) => res.data);
};
