import API from "../../api";

export interface IElectionRound {
  id: string;
  countryCode: string;
  countryName: string;
  countryFullName: string;
  startDate: string;
  title: string;
}

interface IElectionEventsResponse {
  electionRounds: IElectionRound[];
}

export const getCitizenElectionEvents = (): Promise<IElectionEventsResponse> => {
  return API.get("/election-rounds:citizen-report").then((res) => res.data);
};
