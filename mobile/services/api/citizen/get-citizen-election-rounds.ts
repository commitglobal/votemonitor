import API from "../../api";

export interface ICitizenElectionRound {
  id: string;
  countryCode: string;
  countryName: string;
  countryFullName: string;
  startDate: string;
  title: string;
}

export const getCitizenElectionRounds = (): Promise<ICitizenElectionRound[]> => {
  console.log("👀 getCitizenElectionRounds");
  return API.get("/election-rounds:citizen-report").then((res) => res.data.electionRounds);
};
