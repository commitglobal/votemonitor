import API from "../../api";
import { ApiFormAnswer } from "../../interfaces/answer.type";

export type CitizenReportFormAPIPayload = {
  electionRoundId: string;
  citizenReportId: string;
  formId: string;
  locationId: string;
  answers: ApiFormAnswer[];
};

export const postCitizenReportForm = ({
  electionRoundId,
  ...data
}: CitizenReportFormAPIPayload) => {
  return API.post(`/election-rounds/${electionRoundId}/citizen-reports`, data).then(
    (res) => res.data,
  );
};
